using Microsoft.AspNetCore.Identity;

namespace WebTodoList.Data.Identity;

public class RoleEntity : IdentityRole<int>
{
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}
