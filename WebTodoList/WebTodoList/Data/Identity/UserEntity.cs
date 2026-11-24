using Microsoft.AspNetCore.Identity;

namespace WebTodoList.Data.Identity;

public class UserEntity : IdentityUser<int>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? SecondName { get; set; }
    public string? Image { get; set; }
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}
