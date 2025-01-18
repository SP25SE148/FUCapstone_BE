using System.Security.Claims;
using Identity.API.Interfaces;
using Identity.API.Payloads.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

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
        if (context.SecurityToken is JsonWebToken accessToken)
        {
            var requestToken = accessToken.EncodedToken.ToString();

            var emailKey = accessToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
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
