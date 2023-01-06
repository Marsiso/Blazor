using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blazor.Shared.Entities.Utilities;

namespace Blazor.Shared.Entities.Models;

[Table("ResetPasswordRequests")]
public sealed class ResetPasswordRequestEntity
{
    [Column("pk_reset_password_request"), Key]
    public int Id { get; set; }

    [Column("issue_date")]
    [Required(ErrorMessage = "Issue date is a required field")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for issue date")]
    public DateTime IssueDate { get; set; } = DateTime.Now;
    
    [Column("expiration_date")]
    [Required(ErrorMessage = "Expiration date is a required field")]
    [DataType(DataType.DateTime, ErrorMessage = "Invalid data type for reset expiration date")]
    public DateTime ExpirationDate { get; set; } = DateTime.Now.AddMinutes(10);

    [Column("request_code")]
    [Required(ErrorMessage = "Request code is a required field")]
    [DataType(DataType.Text, ErrorMessage = "Invalid data type for reset password code")]
    public string Code { get; set; } = ResetPasswordCodeGenerator.GetUniqueKey(255);
    
    [Column("old_password")]
    [DataType(DataType.Text, ErrorMessage = "Invalid data type for old password")]
    public string OldPassword { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public UserEntity User { get; set; }
}