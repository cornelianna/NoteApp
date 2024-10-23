using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoteApp.Models;
using NoteApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext and Identity services
builder.Services.AddDbContext<NoteAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("NoteAppContext")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<NoteAppContext>();

builder.Services.AddRazorPages();

// Add authentication middleware
builder.Services.AddAuthentication();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

// Map Razor Pages for built-in Identity pages
app.MapRazorPages();

app.Run();
