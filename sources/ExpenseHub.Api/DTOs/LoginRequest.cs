using System.ComponentModel.DataAnnotations;

namespace ExpenseHub.Api.DTOs;

/// <summary>Credentials for authenticating an existing user.</summary>
public class LoginRequest
{
    /// <summary>Gets or sets the user email address.</summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user password.</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
