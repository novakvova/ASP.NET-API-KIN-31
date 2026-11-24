using Microsoft.EntityFrameworkCore;
using WebTodoList.Data;

var builder = WebApplication.CreateBuilder(args);

string strConn = builder.Configuration.GetConnectionString("MyConnectionTodoList");
builder.Services.AddDbContext<AppTodoDbContext>(opt =>
    opt.UseNpgsql(strConn));

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


app.Run();
