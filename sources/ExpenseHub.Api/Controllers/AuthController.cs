using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExpenseHub.Api.DTOs;
using ExpenseHub.Api.Entities;
using ExpenseHub.Api.Responses;
using ExpenseHub.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseHub.Api.Controllers;

/// <summary>Handles user registration and authentication.</summary>
[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    /// <summary>Initializes a new instance of the <see cref="AuthController"/> class.</summary>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    /// <summary>Creates a new user account without assigning any privileged role.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true
        };

        IdentityResult result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return ValidationProblem(new ValidationProblemDetails(
                result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description })));
        }

        return Ok(new { id = user.Id, email = user.Email });
    }

    /// <summary>Authenticates a user and returns a signed JWT bearer token.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid credentials.", Status = 401 });
        }

        Microsoft.AspNetCore.Identity.SignInResult signIn =
            await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (!signIn.Succeeded)
        {
            return Unauthorized(new ProblemDetails { Title = "Invalid credentials.", Status = 401 });
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = _tokenService.CreateToken(user, roles);

        return Ok(new LoginResponse { Token = token });
    }
}
