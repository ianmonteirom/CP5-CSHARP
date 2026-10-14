using System;
using System.Threading.Tasks;
using ExpenseHub.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ExpenseHub.Api.Data;

/// <summary>Provides idempotent seed data initialization for roles and the admin user.</summary>
public static class SeedData
{
    /// <summary>All required role names for this application.</summary>
    public static readonly string[] Roles = ["Employee", "Approver", "Finance", "Auditor", "Admin"];

    private const string AdminEmail = "admin@expensehub.local";

    /// <summary>Creates required roles and the initial admin account when they do not yet exist.</summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider, string adminPassword)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = serviceProvider.GetRequiredService<ILogger<AppDbContext>>();

        foreach (string role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("Admin initial password is not configured. Admin user was not created.");
            return;
        }

        ApplicationUser? existing = await userManager.FindByEmailAsync(AdminEmail);
        if (existing is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            EmailConfirmed = true
        };

        IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            logger.LogInformation("Admin user created successfully.");
        }
        else
        {
            foreach (IdentityError error in result.Errors)
            {
                logger.LogError("Failed to create admin: {Error}", error.Description);
            }
        }
    }
}
