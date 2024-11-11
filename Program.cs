using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoteApp.Data;
using NoteApp.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information() // Levels: Trace < Debug < Information < Warning < Error < Fatal
    .WriteTo.Console() // Console output
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log"); // File output with timestamped filename

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger); 

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext and Identity services
builder.Services.AddDbContext<NoteAppContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("NoteAppContext")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<NoteAppContext>();

// Add authentication with custom login path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Path to custom login page
});

builder.Services.AddRazorPages();

// Add authentication middleware
builder.Services.AddAuthentication();

// Register the repositories
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

Log.Information("Current Environment: {Environment}", app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.

// Always use the exception handler middleware
app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
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
