using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using testPronia.DAL;
using testPronia.Models;
using testPronia.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer("server=DESKTOP-CCR3N2B;database=Pronia;trusted_connection=true;integrated security=true;encrypt=false;"));

builder.Services.AddIdentity<AppUser, IdentityRole>(option =>
{
	option.Password.RequiredLength = 8;
	option.Password.RequireNonAlphanumeric = false;
	option.Password.RequireDigit = true;
	option.Password.RequireUppercase = true;
	option.Password.RequireLowercase = true;

	option.User.RequireUniqueEmail = true;

	option.Lockout.MaxFailedAccessAttempts = 3;
	option.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(3);

});

builder.Services.AddScoped<LayoutService>();
var app = builder.Build();


app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute("default", "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");

app.Run();
