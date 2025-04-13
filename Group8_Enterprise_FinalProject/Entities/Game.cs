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
        [Required]
        public int? TournamentId { get; set; }

        //Tournament Reference
        public Tournament Tournament { get; set; } = null!;

        //Teams list (size must be exactly 2, if size is 0, teams are TBD)
        [Required(ErrorMessage = "Please add the teams")]
        [MaxLength(2, ErrorMessage = "There must be exactly 2 teams")]
        public ICollection<Team> Teams { get; } = new List<Team>();

        public bool AreTeamsDistinct()
        {
            return Teams.Count == 2 && Teams.ElementAt(0).TeamId != Teams.ElementAt(1).TeamId;
        }

        //Method to determine which team won
        public int? GetWinningTeam()
        {
            if (string.IsNullOrEmpty(Result))
            {
                return null; // No result available
            }
            var scores = Result.Split('-');
            if (scores.Length != 2)
            {
                return null; // Invalid result format
            }
            if (int.TryParse(scores[0], out int score1) && int.TryParse(scores[1], out int score2))
            {
                if (score1 > score2)
                {
                    return Teams.ElementAt(0).TeamId; // Team 1 wins
                }
                else if (score2 > score1)
                {
                    return Teams.ElementAt(1).TeamId; // Team 2 wins
                }
                else
                {
                    return null; // Tie
                }
            }
            return null; // Invalid score format
        }
    }
}
