namespace ExpenseHub.Api.Entities;

/// <summary>Category used to group expense reimbursements.</summary>
public class ExpenseCategory
{
    /// <summary>Gets or sets the category identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the category name.</summary>
    public string Name { get; set; } = string.Empty;
}
