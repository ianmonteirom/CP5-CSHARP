using System.ComponentModel.DataAnnotations;

namespace ExpenseHub.Api.DTOs;

/// <summary>Payload for creating a new user account.</summary>
public class RegisterRequest
{
    /// <summary>Gets or sets the user email address.</summary>
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the user password.</summary>
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
