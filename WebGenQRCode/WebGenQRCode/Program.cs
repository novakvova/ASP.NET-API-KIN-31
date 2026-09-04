using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebGenQRCode.Data;
using WebGenQRCode.Data.Entities.Identity;
using WebGenQRCode.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppQrDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("QrConnection")));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppQrDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddSwaggerGen(); //Додаємо swagger - кажемо, що він є

builder.Services.AddControllers();

const string reactCorsPolicy = "ReactClient";

var reactCorsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(reactCorsPolicy, policy =>
    {
        policy.WithOrigins(reactCorsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(reactCorsPolicy); //дозволяємо використання cors правил

app.UseSwagger(); //Використай Swagger
app.UseSwaggerUI(); //Додай графічний інтерфейс

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await app.SeedData();

app.Run();
