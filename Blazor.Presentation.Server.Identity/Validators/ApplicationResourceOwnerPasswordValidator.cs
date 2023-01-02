using System.Security.Claims;
using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace Blazor.Presentation.Server.Identity.Validators;

public sealed class ApplicationResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IRepositoryManager _repository;

    public ApplicationResourceOwnerPasswordValidator(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var user = await _repository.User.GetUserAsync(context.UserName, false);
            if (user is not null)
            {
                if (user.Password == context.Password)
                {
                    context.Result = new GrantValidationResult(
                        subject: user.Id.ToString(),
                        authenticationMethod: "custom",
                        claims: GetUserClaims(user));
                    
                    return;
                }

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password");
                return;
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist");
        }
        catch (Exception)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
        }
    }

    internal static IList<Claim> GetUserClaims(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new("user_id", user.Id.ToString()),
            new(JwtClaimTypes.Email, user.Email),
            new(JwtClaimTypes.Name, !String.IsNullOrEmpty(user.FirstName) && !String.IsNullOrEmpty(user.LastName) ? String.Format("{0} {1}", user.FirstName, user.LastName) : String.Empty),
            new(JwtClaimTypes.GivenName, user.FirstName ?? String.Empty),
            new(JwtClaimTypes.FamilyName, user.LastName ?? String.Empty),
            new(JwtClaimTypes.Address, user.Address),
            new("country", user.Country),
            new("is_active", user.IsActive.ToString())
        };
        
        claims.AddRange(user.Roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
        
        return claims;
    }
}