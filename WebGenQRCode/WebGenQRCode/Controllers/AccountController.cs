using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebGenQRCode.Constants;
using WebGenQRCode.Data.Entities.Identity;
using WebGenQRCode.Interfaces;
using WebGenQRCode.Models.Account;

namespace WebGenQRCode.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController(IImageService imageService,
    UserManager<UserEntity> userManager,
    IAccountService accountService,
    IJwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
                throw new Exception("Дана пошта уже зареєстрована");
            user = new UserEntity
            {
                Email = model.Email,
                UserName = model.Email,
                LastName = model.LastName,
                FirstName = model.FirstName
            };
            if (model.ImageFile != null)
                user.Image = await imageService.SaveOptimizedImageAsync(model.ImageFile);
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            await userManager.AddToRoleAsync(user, Roles.User);

            var token = await jwtTokenService.CreateTokenAsync(user);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }

    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if(user!=null && await userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = await jwtTokenService.CreateTokenAsync(user);
            return Ok(new { Token = token });
        }

        return Unauthorized("Не вірно вказано дані");
    }

    [HttpPost]
    public async Task<IActionResult> LoginByGoogle([FromBody] GoogleLoginRequestModel model)
    {
        try
        {
            var token = await accountService.LoginByGoogle(model.Token);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email");
        if(string.IsNullOrEmpty(email))
            return Unauthorized("Email not found in token");

        var user = await userManager.FindByEmailAsync(email);
        if(user == null)
            return NotFound("User not found");
        var roles = await userManager.GetRolesAsync(user);
        var model = new ProfileModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Image = user.Image ?? string.Empty,
            Roles = roles
        };
        return Ok(model);
    }

}
