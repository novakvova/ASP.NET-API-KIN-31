using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebGenQRCode.Constants;
using WebGenQRCode.Data;
using WebGenQRCode.Data.Entities.Identity;
using WebGenQRCode.Models.Seeder;
using WebGenQRCode.Interfaces;

namespace WebGenQRCode.Extensions;

public static class DbSeeder
{
    //This - Розширення класу WebApplication
    public static async Task SeedData(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        //Цей об'єкт буде верта посилання на конткетс, який зараєстрвоано в Progran.cs
        var context = scope.ServiceProvider.GetRequiredService<AppQrDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();

        context.Database.Migrate();

        if (!context.Roles.Any())
        {
            foreach (var roleName in Roles.ListRoles())
            {
                await roleManager.CreateAsync(new RoleEntity { Name = roleName });
            }
        }

        if (!context.Users.Any())
        {
            var curDir = Directory.GetCurrentDirectory();
            var jsonFile = Path.Combine(curDir, "Helpers", "JsonData", "Users.json");
            if (File.Exists(jsonFile))
            {
                var jsonData = await File.ReadAllTextAsync(jsonFile);
                try
                {
                    int i = 1;
                    //Список користувачів, які ми отримали із файлу
                    var users = JsonSerializer.Deserialize<List<SeederUserModel>>(jsonData);
                    foreach(var user in users)
                    {
                        var entity = new UserEntity
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            UserName = user.Email   
                        };

                        if (!string.IsNullOrEmpty(user.Image))
                        {
                            entity.Image = await imageService.SaveImageFromUrlAsync(user.Image);
                        }

                        var result = await userManager.CreateAsync(entity, user.Password);
                        if(result.Succeeded)
                        {
                            foreach (var role in user.Roles)
                                await userManager.AddToRoleAsync(entity, role);
                        }
                        Console.WriteLine($"Додано {i} користувачів");
                        i++;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Виклик помилки при Seed Users {0}", ex.Message);
                }
            }
        }
    }
}
