using ExpenseHub.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseHub.Api.Data;

/// <summary>Entity Framework Core database context for ExpenseHub.</summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>Initializes a new instance of the <see cref="AppDbContext"/> class.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the expense reimbursement requests.</summary>
    public DbSet<Expense> Expenses { get; set; } = null!;

    /// <summary>Gets or sets the expense categories.</summary>
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; } = null!;

    /// <summary>Gets or sets the expense audit history entries.</summary>
    public DbSet<ExpenseHistory> ExpenseHistories { get; set; } = null!;

    /// <summary>Gets or sets the payment records.</summary>
    public DbSet<PaymentRecord> PaymentRecords { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.OwnerId).IsRequired();

            entity.HasOne(e => e.Owner)
                  .WithMany()
                  .HasForeignKey(e => e.OwnerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryId)
                  .IsRequired(false);
        });

        builder.Entity<ExpenseHistory>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.Property(h => h.ActorId).IsRequired();

            entity.HasOne(h => h.Expense)
                  .WithMany(e => e.History)
                  .HasForeignKey(h => h.ExpenseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.Actor)
                  .WithMany()
                  .HasForeignKey(h => h.ActorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => p.ExpenseId).IsUnique();
            entity.Property(p => p.ActorId).IsRequired();

            entity.HasOne(p => p.Expense)
                  .WithOne(e => e.PaymentRecord)
                  .HasForeignKey<PaymentRecord>(p => p.ExpenseId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Actor)
                  .WithMany()
                  .HasForeignKey(p => p.ActorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
