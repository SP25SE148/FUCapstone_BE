﻿namespace Identity.API.Payloads.Responses;

public class Authenticated
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
