using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseHub.Api.DTOs;

/// <summary>Payload for assigning or replacing a user's roles.</summary>
public class AssignRolesRequest
{
    /// <summary>Gets or sets the complete set of roles to assign to the user.</summary>
    [Required]
    public IReadOnlyList<string> Roles { get; set; } = [];
}
