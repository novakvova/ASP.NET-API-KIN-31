using Microsoft.AspNetCore.Identity;

namespace WebTodoList.Data.Identity;

public class UserRoleEntity : IdentityUserRole<int>
{
    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}
