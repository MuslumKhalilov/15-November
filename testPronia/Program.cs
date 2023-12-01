using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("server=DESKTOP-CCR3N2B;database=Pronia;trusted_connection=true;integrated security=true;encrypt=false;"));
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("default", "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");

app.Run();
