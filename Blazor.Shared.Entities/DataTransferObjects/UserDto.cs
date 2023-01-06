using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.DataTransferObjects;

public sealed class UserDto
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Password { get; set; }

    public bool IsActive { get; set; } = true;

    public string Address { get; set; }
    
    public string Country { get; set; }

    public string[] Roles { get; set; } = { Identity.Roles.Visitor };
}