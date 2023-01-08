namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class ResetPasswordRequestDto
{
    public int Id { get; set; }

    public DateTime IssueDate { get; set; } 
    
    public DateTime ExpirationDate { get; set; }

    public string Code { get; set; } 
    public string OldPassword { get; set; } 
    
    public UserDto User { get; set; }
}