using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FUC.Common.Payloads;
using Identity.API.Interfaces;
using Identity.API.Models;
using Identity.API.Payloads.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Controllers;

public class AuthController(UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    ICacheService cacheService,
    ILogger<AuthController> logger) : ApiController
{

    [HttpPost("login")] //Post: api/auth/login
    public async Task<ActionResult<Authenticated>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogError("User with Email:{Email} does not exist", request.Email);
            return Unauthorized("User does not exist");
        }

        var result = await userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
            return Unauthorized("Invalid password");

        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.GivenName, user.UserCode),
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
        };

        var roles = await userManager.GetRolesAsync(user);

        userClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var authenticationResponse = new Authenticated
        {
            AccessToken = jwtTokenService.GenerateAccessToken(userClaims),
            RefreshToken = jwtTokenService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.Now.AddDays(5)
        };

        // Add the token into whitelist
        await cacheService.SetAsync(request.Email, authenticationResponse, cancellationToken);

        return Ok(authenticationResponse);
    }

    [HttpPost("token/refresh")]
    public async Task<ActionResult<Authenticated>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);

        var emailKey = principal.FindFirstValue(ClaimTypes.Email) ?? throw new SecurityTokenException("Invalid Token");
      
        var authenticated = await cacheService.GetAsync<Authenticated>(emailKey, cancellationToken);

        if (authenticated == null || authenticated.RefreshToken != request.RefreshToken || authenticated.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new SecurityTokenException("Invalid Token");
        }

        var newAuthenticated = new Authenticated
        {
            AccessToken = jwtTokenService.GenerateAccessToken(principal.Claims
                .Where(x => x.Type != JwtRegisteredClaimNames.Aud)
                .ToList()),
            RefreshToken = jwtTokenService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.Now.AddDays(5)
        };

        await cacheService.SetAsync(emailKey, newAuthenticated, cancellationToken);

        return Ok(newAuthenticated);
    }

    [HttpPost("token/revoke")]
    public async Task<ActionResult<Authenticated>> RevokeToken([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);

        var emailKey = principal.FindFirstValue(ClaimTypes.Email) ?? throw new SecurityTokenException("Invalid Token");

        var authenticated = await cacheService.GetAsync<Authenticated>(emailKey, cancellationToken);

        if (authenticated is null)
        {
            logger.LogError("Can not get value from Redis");
            throw new SecurityTokenException("Can not get value from Redis");
        }

        await cacheService.RemoveAsync(emailKey, cancellationToken);

        return Ok();
    }
}
