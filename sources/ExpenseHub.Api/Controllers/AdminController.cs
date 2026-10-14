using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpenseHub.Api.Data;
using ExpenseHub.Api.DTOs;
using ExpenseHub.Api.Entities;
using ExpenseHub.Api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseHub.Api.Controllers;

/// <summary>Admin-only operations for user and role management.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>Initializes a new instance of the <see cref="AdminController"/> class.</summary>
    public AdminController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

   

    /// <summary>Replaces a user's roles with the provided set; protects the Admin's own admin role.</summary>
    [HttpPut("users/{id}/roles")]
    public async Task<ActionResult<UserResponse>> AssignRoles(
        string id,
        [FromBody] AssignRolesRequest request,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return NotFound(new ProblemDetails { Title = "User not found.", Status = 404 });
        }

        foreach (string role in request.Roles)
        {
            if (!SeedData.Roles.Contains(role))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = $"Unknown role: {role}.",
                    Status = 400
                });
            }
        }

        string? currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == user.Id && !request.Roles.Contains("Admin"))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "An admin cannot remove their own Admin role.",
                Status = 400
            });
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRolesAsync(user, request.Roles);

        IList<string> updatedRoles = await _userManager.GetRolesAsync(user);

        return Ok(new UserResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Roles = updatedRoles.ToList().AsReadOnly()
        });
    }
}
