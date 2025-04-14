using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Group8_Enterprise_FinalProject.Models;
using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Services;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

var hangfireConnStr = builder.Configuration.GetConnectionString("HangfireConnectionDB");
// Configure Hangfire to use SQL Server storage (persists task information between runs of the app)
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(hangfireConnStr);
});

// Adding our custom service as a scoped service here
builder.Services.AddScoped<ITournamentManagerService, TournamentManagerService>();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

// Setting up paths to vies for logging in and access denied, which user will be sent to for unauthorized actions
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/LogIn";          
    options.AccessDeniedPath = "/Account/AccessDenied"; 
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add CORS support for our API (basic policy, nothing crazy for this)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowTournamentClients", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Getting our main database string and adding it as a context here
var connStr = builder.Configuration.GetConnectionString("ETourneyProDB");
builder.Services.AddDbContext<TournamentDbContext>(options => options.UseSqlServer(connStr));

// Code to setup identity services, requirements for passwords (SAME AS WHAT WE DID IN OUR QUIZZES AND EXAMPLE CODE IN CLASS)
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
}).AddEntityFrameworkStores<TournamentDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use cors policy here
app.UseCors("AllowTournamentClients");

app.UseAuthentication(); // Using authentication (added BEFORE authorization)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tournament}/{action=GetAllTournaments}/{id?}")
    .WithStaticAssets();


// Calling static method to create Admin (Organizer) user
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await TournamentDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.Run();
