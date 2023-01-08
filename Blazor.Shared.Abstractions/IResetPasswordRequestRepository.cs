using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;

namespace Blazor.Shared.Abstractions;

public interface IResetPasswordRequestRepository
{
    Task<PagedList<ResetPasswordRequestEntity>> GetAllPasswordResetRequestsAsync(
        ResetPasswordRequestParameters resetPasswordRequestParameters, 
        bool trackChanges);
    Task<ResetPasswordRequestEntity> GetPasswordResetRequestAsync(int passwordResetRequestId, bool trackChanges);
    Task<ResetPasswordRequestEntity> GetPasswordResetRequestAsync(string passwordResetRequestCode, bool trackChanges);
    void CreatePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest);
    void UpdatePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest);
    void DeletePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest);
}