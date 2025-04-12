using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    public class Tournament
    {
        //PK
        public int TournamentId { get; set; }

        //Name (required)
        [Required(ErrorMessage = "Please enter a tournament name")]
        public string? Name { get; set; }

        //Game (required)
        [Required(ErrorMessage = "Please enter a game")]
        public string? Game { get; set; }

        //Number of players per team (required)
        [Required(ErrorMessage = "Please enter the number of players per team")]
        public int NumPlayersPerTeam { get; set; }

        //Start date and time (required)
        [Required(ErrorMessage = "Please enter the start date and time")]
        public DateTime StartDateTime { get; set; }

        //Number of games to be played (required)
        [Required(ErrorMessage = "Please enter the number of games to be played")]
        public int NumGames { get; set; }
    }
}
