using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    //Has a two-to-one relationship with Game
    //Has a one-to-many relationship with Player
    //Has a many-to-one relationship with Tournament
    public class Team
    {
        //PK
        public int TeamId { get; set; }

        //Team Name (required)
        [Required(ErrorMessage = "Please enter a team name")]
        public string? Name { get; set; }

        //Win-Loss-Tie record (required)
        [Required(ErrorMessage = "Please enter the win-loss-tie record")]
        public int[] Record { get; set; } = new int[3]; // [Wins, Losses, Ties]

        //Players list (size must be exactly NumPlayersPerTeam)
        [Required(ErrorMessage = "Please add the players")]
        public ICollection<Player> Players { get; } = new List<Player>();

        //FK
        public int GameId { get; set; }

        //Game Reference
        public Game Game { get; set; } = null!;

        //TournamentId (FK)
        public int TournamentId { get; set; }

        //Tournament Reference
        public Tournament Tournament { get; set; } = null!;

        // Validation method to check the number of players
        public bool ValidatePlayerCount()
        {
            return Players.Count == Tournament.NumPlayersPerTeam;
        }
    }
}
