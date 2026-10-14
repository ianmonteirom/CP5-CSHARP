using System.ComponentModel.DataAnnotations;

namespace ExpenseHub.Api.DTOs;

/// <summary>Payload for rejecting a submitted expense with a mandatory justification.</summary>
public class RejectExpenseRequest
{
    /// <summary>Gets or sets the rejection justification.</summary>
    [Required]
    [StringLength(500, MinimumLength = 10)]
    public string Justification { get; set; } = string.Empty;
}
