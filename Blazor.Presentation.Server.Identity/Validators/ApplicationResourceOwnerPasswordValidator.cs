using Blazor.Shared.Abstractions;
using IdentityServer4.Validation;

namespace Blazor.Presentation.Server.Identity.Validators;

public sealed class ApplicationResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IRepositoryManager _repository;

    public ApplicationResourceOwnerPasswordValidator(IRepositoryManager repository)
    {
        _repository = repository;
    }

    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}