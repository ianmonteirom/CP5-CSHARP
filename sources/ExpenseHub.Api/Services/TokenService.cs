using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseHub.Api.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseHub.Api.Services;

/// <summary>Creates signed JWT bearer tokens using configuration-supplied keys.</summary>
public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    /// <summary>Initializes a new instance of the <see cref="TokenService"/> class.</summary>
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc/>
    public string CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        string key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT signing key is not configured (Jwt:Key).");

        string issuer = _configuration["Jwt:Issuer"] ?? "ExpenseHub";
        string audience = _configuration["Jwt:Audience"] ?? "ExpenseHub";
        int expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out int hours) ? hours : 24;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
