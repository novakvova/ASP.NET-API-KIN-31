using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebTodoList.Data;
using WebTodoList.Data.Identity;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration.GetConnectionString("MyConnectionTodoList");
builder.Services.AddDbContext<AppTodoDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppTodoDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using(var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetService<UserManager<UserEntity>>();

    var roleManager = scope.ServiceProvider
        .GetService<RoleManager<RoleEntity>>();
    
    if(roleManager.Roles.Any() == false)
    {
        string[] roles = { "Admin", "User" };
        foreach (string name in roles)
        {
            RoleEntity role = new RoleEntity
            {
                Name = name
            };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
                Console.WriteLine($"--Роль {name} успішно створено--");
            else
                Console.WriteLine($"!!!Помилка створення ролі {name}!!!");
        }

    }
}

app.Run();
