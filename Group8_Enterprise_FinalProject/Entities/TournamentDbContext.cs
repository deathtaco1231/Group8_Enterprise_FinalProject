using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Group8_Enterprise_FinalProject.Models;

namespace Group8_Enterprise_FinalProject.Entities
{
    /// <summary>
    /// This class will inherit from the Entity Framework (EF) class
    /// called DbContext and is used by the code to interact with the DB
    /// </summary>
    public class TournamentDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Define a constructor that simply passes the options argument
        /// up to the base class constuctor
        /// </summary>
        /// <param name="options"></param>
        public TournamentDbContext(DbContextOptions options)
            : base(options)
        {
        }


        /// <summary>
        /// Add some records upon build
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Calling base method as specified in quiz instructions
            // TODO seed database with data
        }

        /// <summary>
        /// Creates an Admin user
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string username = "DefaultOrganizer";
            string password = "Cheeseburger1234#";
            string roleName = "Organizer";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

    }
}
