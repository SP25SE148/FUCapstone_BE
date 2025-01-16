using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.API.Interfaces;
using Identity.API.Models;
using Identity.API.Payloads.Requests;
using Identity.API.Payloads.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(UserManager<ApplicationUser> userManager, 
    IJwtTokenService jwtTokenService, 
    ICacheService cacheService, 
    ILogger<AuthController> logger) : ControllerBase
{

    [HttpPost("login")] //Post: api/auth/login
    public async Task<ActionResult<Authenticated>> Login([FromBody]LoginRequest request)
    {
        var user = await userManager.Users
            .SingleOrDefaultAsync(x => x.Email == request.Email);

        if (user is null)
        {
            logger.LogInformation($"User with Email:{request.Email} does not exist");
            return Unauthorized("User does not exist");
        }

        var result = await userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
            return Unauthorized("Invalid password");

        var userClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, request.Email),
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

        await cacheService.SetAsync(request.Email, authenticationResponse);

        return Ok(authenticationResponse);
    }

    [HttpPost("token/refresh")]
    public async Task<ActionResult<Authenticated>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);

        var emailKey = principal.FindFirstValue(JwtRegisteredClaimNames.Email);

        var authenticated = await cacheService.GetAsync<Authenticated>(emailKey, cancellationToken);

        if (authenticated == null || authenticated.RefreshToken != request.RefreshToken || authenticated.RefreshTokenExpiryTime <= DateTime.Now) 
        {
            return BadRequest("Request token invalid!");
        }

        var newAuthenticated = new Authenticated
        {
            AccessToken = jwtTokenService.GenerateAccessToken(principal.Claims),
            RefreshToken = jwtTokenService.GenerateRefreshToken(),
            RefreshTokenExpiryTime = DateTime.Now.AddDays(5)
        };

        await cacheService.SetAsync(emailKey, newAuthenticated);

        return Ok(newAuthenticated);
    }

    [HttpPost("token/revoke")]
    public async Task<ActionResult<Authenticated>> RevokeToken([FromBody] string acceccToken, CancellationToken cancellationToken)
    {
        var principal = jwtTokenService.GetPrincipalFromExpiredToken(acceccToken);

        var emailKey = principal.FindFirstValue(JwtRegisteredClaimNames.Email);

        var authenticated = await cacheService.GetAsync<Authenticated>(emailKey);
            
        if(authenticated is null)
        {
            logger.LogError("Can not get value from Redis");
            throw new Exception("Can not get value from Redis");
        }

        await cacheService.RemoveAsync(emailKey, cancellationToken);

        return Ok();
    }
}
