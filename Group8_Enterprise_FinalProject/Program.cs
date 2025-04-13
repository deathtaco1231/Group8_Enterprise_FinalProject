using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Group8_Enterprise_FinalProject.Models;
using Group8_Enterprise_FinalProject.Entities;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

var hangfireConnStr = builder.Configuration.GetConnectionString("HangfireConnectionDB");
// Configure Hangfire to use SQL Server storage (persists task information between runs of the app)
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(hangfireConnStr);
});

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add CORS support for our API (basic policy, nothing crazy for this)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowTournamentClients", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var connStr = builder.Configuration.GetConnectionString("ETourneyProDB");
builder.Services.AddDbContext<TournamentDbContext>(options => options.UseSqlServer(connStr));

// Code to setup identity services, requirements for passwords
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Calling static method to create Admin (Organizer) user
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await TournamentDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.Run();
