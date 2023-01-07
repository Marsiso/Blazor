using Blazor.Shared.Abstractions;
using Blazor.Shared.Entities.DbContexts;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Entities.RequestFeatures;
using Blazor.Shared.Implementations.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Blazor.Shared.Implementations.Repositories;

public sealed class ResetPasswordRequestRepository : RepositoryBase<ResetPasswordRequestEntity>, IResetPasswordRequestRepository
{
    public ResetPasswordRequestRepository(SqlContext context) : base(context)
    {
    }

    public async Task<PagedList<ResetPasswordRequestEntity>> GetAllPasswordResetRequestsAsync(ResetPasswordRequestParameters resetPasswordRequestParameters, bool trackChanges)
    {
        var resetPasswordRequests = await FindAll(trackChanges)
            .FilterResetPasswordRequests(resetPasswordRequestParameters.MinIssueDate, 
                resetPasswordRequestParameters.MaxIssueDate,
                resetPasswordRequestParameters.MinExpirationDate,
                resetPasswordRequestParameters.MaxExpirationDate)
            .SearchResetPasswordRequests(resetPasswordRequestParameters.SearchTerm)
            .SortResetPasswordRequests(resetPasswordRequestParameters.OrderBy)
            .ToListAsync();
        
        return PagedList<ResetPasswordRequestEntity>.ToPagedList(resetPasswordRequests, 
            resetPasswordRequestParameters.PageNumber, 
            resetPasswordRequestParameters.PageSize);
    }

    public async Task<ResetPasswordRequestEntity> GetPasswordResetRequestAsync(int passwordResetRequestId, bool trackChanges) =>
        await FindByCondition(passwordResetRequest => passwordResetRequest.Id == passwordResetRequestId, trackChanges)
            .SingleOrDefaultAsync();
    
    public async Task<ResetPasswordRequestEntity> GetPasswordResetRequestAsync(int userId, string passwordResetRequestCode, bool trackChanges) =>
        await FindByCondition(passwordResetRequest => 
                passwordResetRequest.UserId == userId && 
                passwordResetRequest.Code == passwordResetRequestCode, trackChanges)
            .SingleOrDefaultAsync();

    public void CreatePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest) =>
        Create(passwordResetRequest);

    public void UpdatePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest) =>
        Update(passwordResetRequest);

    public void DeletePasswordResetRequest(ResetPasswordRequestEntity passwordResetRequest) =>
        Delete(passwordResetRequest);
}