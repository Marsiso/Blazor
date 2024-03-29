﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Users")]
public sealed class UserEntity
{
    [Column("pk_user"), Key] 
    public int Id { get; set; }

    [Column("email")]
    [Required(ErrorMessage = "Email is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the email is 50 characters")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Invalid email address format")]
    public string Email { get; set; }

    [Column("name")]
    [Required(ErrorMessage = "Name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Name must be text value type")]
    public string Name { get; set; }

    [Column("first_name")]
    [Required(ErrorMessage = "First name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the first name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "First name must be text value type")]
    public string FirstName { get; set; }

    [Column("last_name")]
    [Required(ErrorMessage = "Last name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the last name is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Last name must be text value type")]
    public string LastName { get; set; }

    [Column("password")]
    [Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image alternative name is 50 characters")]
    [DataType(DataType.Password, ErrorMessage = "Invalid password format")]
    public string Password { get; set; }

    [Column("is_active"), Required(ErrorMessage = "User activity status is a required field")]
    public bool IsActive { get; set; } = true;

    [Column("address")]
    [Required(ErrorMessage = "Address is a required field")]
    [MaxLength(100, ErrorMessage = "Maximum length for the address is 100 characters")]
    [DataType(DataType.Text, ErrorMessage = "Address must be text value type")]
    public string Address { get; set; }
    
    [Column("country")]
    [Required(ErrorMessage = "Country is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the country is 50 characters")]
    [DataType(DataType.Text, ErrorMessage = "Country must be text value type")]
    public string Country { get; set; }

    [Column("roles"), Required(ErrorMessage = "Image alternative name is a required field")]
    [MaxLength(50, ErrorMessage = "Maximum length for the image alternative name is 50 characters")]
    //[RegularExpression("admin|manager|visitor", ErrorMessage = "Invalid user role")]
    public string[] Roles { get; set; } = { Identity.Roles.Visitor };
}