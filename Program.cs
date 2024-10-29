using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using NoteApp.Repositories;
<<<<<<< HEAD
using Serilog;
=======
>>>>>>> origin/anna-test

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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

// Register the IPostRepository service
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>(); 
<<<<<<< HEAD
=======

>>>>>>> origin/anna-test

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Optional: Configure error handling for production environment
    // app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();