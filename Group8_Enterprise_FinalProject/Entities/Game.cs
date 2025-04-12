using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    //Has a many-to-one relationship with Tournament
    //Has a one-to-two relationship with Team
    public class Game
    {
        //PK
        public int GameId { get; set; }

        //Game Date and Time (required)
        [Required(ErrorMessage = "Please enter the game date and time")]
        public DateTime GameDateTime { get; set; }

        //Game Result (should be in form "Score1-Score2")
        //If empty, set score to 0-0
        public string? Result { get; set; } = "0-0";

        //TournamentId (FK)
        public int TournamentId { get; set; }

        //Tournament Reference
        public Tournament Tournament { get; set; } = null!;

        //Teams list (size must be exactly 2)
        [Required(ErrorMessage = "Please add the teams")]
        [MinLength(2, ErrorMessage = "There must be exactly 2 teams")]
        public ICollection<Team> Teams { get; } = new List<Team>();
    }
}
