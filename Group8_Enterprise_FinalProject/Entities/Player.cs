using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    //Has a many-to-one relationship with Team
    public class Player
    {
        //PK
        public int PlayerId { get; set; }

        //Player Name (required)
        [Required(ErrorMessage = "Please enter a player name")]
        public string? Name { get; set; }

        //TeamId (FK)
        public int TeamId { get; set; }

        //Team Reference
        public Team Team { get; set; } = null!;
    }
}