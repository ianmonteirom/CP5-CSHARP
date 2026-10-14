namespace ExpenseHub.Api.Responses;

/// <summary>Response returned after a successful login.</summary>
public class LoginResponse
{
    /// <summary>Gets or sets the JWT bearer token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Gets or sets the token type (always "Bearer").</summary>
    public string TokenType { get; set; } = "Bearer";
}
