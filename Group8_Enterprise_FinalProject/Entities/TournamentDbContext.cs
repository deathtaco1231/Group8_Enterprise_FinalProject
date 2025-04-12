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

            // Seed the database with some initial tournament data
            modelBuilder.Entity<Tournament>()
                .HasData(
                    new Tournament { TournamentId = 1, Name = "Spring Tournament", Game = "Valorant", NumPlayersPerTeam = 5, StartDateTime = DateTime.Now, NumGames = 1}
                );

            // Seed the database with some initial game data
            modelBuilder.Entity<Game>()
                .HasData(
                    new Game { GameId = 1, GameDateTime = DateTime.Now, Result = "0-0", TournamentId = 1 },
                    new Game { GameId = 2, GameDateTime = DateTime.Now.AddHours(1), Result = "0-0", TournamentId = 1 },
                    new Game { GameId = 3, GameDateTime = DateTime.Now.AddDays(1), Result = "0-0", TournamentId = 1 }
                );

            // Seed the database with some initial team data
            modelBuilder.Entity<Team>()
                .HasData(
                    new Team { TeamId = 1, Record = new int[] { 0, 0, 0 }, Name = "Team A", GameId = 1, TournamentId = 1 },
                    new Team { TeamId = 2, Record = new int[] { 0, 0, 0 }, Name = "Team B", GameId = 1, TournamentId = 1 },
                    new Team { TeamId = 3, Record = new int[] { 0, 0, 0 }, Name = "Team C", GameId = 2, TournamentId = 1 },
                    new Team { TeamId = 4, Record = new int[] { 0, 0, 0 }, Name = "Team D", GameId = 2, TournamentId = 1 }
                );

            // Seed the database with some initial player data
            modelBuilder.Entity<Player>()
                .HasData(
                    new Player { PlayerId = 1, Name = "Player 1", TeamId = 1 },
                    new Player { PlayerId = 2, Name = "Player 2", TeamId = 1 },
                    new Player { PlayerId = 3, Name = "Player 3", TeamId = 1 },
                    new Player { PlayerId = 4, Name = "Player 4", TeamId = 1 },
                    new Player { PlayerId = 5, Name = "Player 5", TeamId = 1 },
                    new Player { PlayerId = 6, Name = "Player 6", TeamId = 2 },
                    new Player { PlayerId = 7, Name = "Player 7", TeamId = 2 },
                    new Player { PlayerId = 8, Name = "Player 8", TeamId = 2 },
                    new Player { PlayerId = 9, Name = "Player 9", TeamId = 2 },
                    new Player { PlayerId = 10, Name = "Player 10", TeamId = 2 },
                    new Player { PlayerId = 11, Name = "Player 11", TeamId = 3 },
                    new Player { PlayerId = 12, Name = "Player 12", TeamId = 3 },
                    new Player { PlayerId = 13, Name = "Player 13", TeamId = 3 },
                    new Player { PlayerId = 14, Name = "Player 14", TeamId = 3 },
                    new Player { PlayerId = 15, Name = "Player 15", TeamId = 3 },
                    new Player { PlayerId = 16, Name = "Player 16", TeamId = 4 },
                    new Player { PlayerId = 17, Name = "Player 17", TeamId = 4 },
                    new Player { PlayerId = 18, Name = "Player 18", TeamId = 4 },
                    new Player { PlayerId = 19, Name = "Player 19", TeamId = 4 },
                    new Player { PlayerId = 20, Name = "Player 20", TeamId = 4 }
                );
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
