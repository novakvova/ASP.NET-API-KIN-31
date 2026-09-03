using Microsoft.EntityFrameworkCore;
using WebGenQRCode.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppQrDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("QrConnection")));

// Add services to the container.
builder.Services.AddSwaggerGen(); //Додаємо swagger - кажемо, що він є

builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger(); //Використай Swagger
app.UseSwaggerUI(); //Додай графічний інтерфейс

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
