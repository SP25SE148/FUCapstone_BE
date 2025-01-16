using System.IdentityModel.Tokens.Jwt;
using Identity.API.Interfaces;
using Identity.API.Payloads.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Identity.API.Attributes;

public class CustomJwtBearerEvents : JwtBearerEvents
{
    private readonly ICacheService _cacheService;

    public CustomJwtBearerEvents(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        if (context.SecurityToken is JwtSecurityToken accessToken)
        {
            var requestToken = accessToken.RawData.ToString();

            var emailKey = accessToken.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Email)?.Value;
            var authenticated = await _cacheService.GetAsync<Authenticated>(emailKey);

            if (authenticated is null || authenticated.AccessToken != requestToken)
            {
                context.Response.Headers.Add("IS-TOKEN-REVOKED", "true");
                context.Fail("Authentication fail. Token has been revoked!");
            }
        }
        else
        {
            context.Fail("Authentication fail.");
        }
    }
}
