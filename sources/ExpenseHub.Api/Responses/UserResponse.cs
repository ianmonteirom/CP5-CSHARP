using System.Collections.Generic;

namespace ExpenseHub.Api.Responses;

/// <summary>Summary representation of an application user.</summary>
public class UserResponse
{
    /// <summary>Gets or sets the user identifier.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the user email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the roles assigned to this user.</summary>
    public IReadOnlyList<string> Roles { get; set; } = [];
}
