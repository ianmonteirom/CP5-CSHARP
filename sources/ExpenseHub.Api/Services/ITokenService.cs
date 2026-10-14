using System.Collections.Generic;
using ExpenseHub.Api.Entities;

namespace ExpenseHub.Api.Services;

/// <summary>Generates bearer tokens for authenticated users.</summary>
public interface ITokenService
{
    /// <summary>Creates a signed JWT for the given user and roles.</summary>
    string CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
