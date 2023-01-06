using System.Linq.Dynamic.Core;
using Blazor.Shared.Entities.Models;
using Blazor.Shared.Implementations.Extensions.Utility;

namespace Blazor.Shared.Implementations.Extensions;

public static class RepositoryResetPasswordRequestExtensions
{
    public static IQueryable<ResetPasswordRequestEntity> FilterResetPasswordRequests
    (
        this IQueryable<ResetPasswordRequestEntity> resetPasswordRequests,
        DateTime minIssueDate,
        DateTime maxIssueDate,
        DateTime minExpirationDate, 
        DateTime maxExpirationDate)
    {
        return resetPasswordRequests.Where(resetPasswordRequest => resetPasswordRequest.IssueDate >= minIssueDate && 
                                                                   resetPasswordRequest.IssueDate <= maxIssueDate &&
                                                                   resetPasswordRequest.ExpirationDate >= minExpirationDate &&
                                                                   resetPasswordRequest.ExpirationDate <= maxExpirationDate);
    }

    public static IQueryable<ResetPasswordRequestEntity> SearchResetPasswordRequests(this IQueryable<ResetPasswordRequestEntity> resetPasswordRequests, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return resetPasswordRequests; 
        }

        var lowerCaseTerm = searchTerm.Trim().ToLower(); 
        return resetPasswordRequests.Where(resetPasswordRequest => resetPasswordRequest.Code.Contains(lowerCaseTerm) || 
                                         resetPasswordRequest.Id.ToString().Contains(lowerCaseTerm) ||
                                         resetPasswordRequest.IssueDate.ToString().Contains(searchTerm) ||
                                         resetPasswordRequest.ExpirationDate.ToString().Contains(searchTerm) ||
                                         resetPasswordRequest.UserId.ToString().Contains(searchTerm));
    }

    public static IQueryable<ResetPasswordRequestEntity> SortResetPasswordRequests(this IQueryable<ResetPasswordRequestEntity> resetPasswordRequests,
        string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return resetPasswordRequests.OrderBy(resetPasswordRequest => resetPasswordRequest.Id);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<ResetPasswordRequestEntity>(orderByQueryString);
        
        return string.IsNullOrWhiteSpace(orderQuery) 
            ? resetPasswordRequests.OrderBy(resetPasswordRequest => resetPasswordRequest.Id) 
            : resetPasswordRequests.OrderBy(orderQuery);
    }
}