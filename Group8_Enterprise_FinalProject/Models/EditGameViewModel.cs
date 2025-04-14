using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Models
{
    public class EditGameViewModel
    {
        public int GameId { get; set; }

        [Required]
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "Game Date & Time is required")]
        [Display(Name = "Game Date & Time")]
        public DateTime GameDateTime { get; set; }

        // We'll edit the team names via simple properties.
        [Required(ErrorMessage = "Team 1 Name is required")]
        [Display(Name = "Team 1 Name")]
        public string Team1Name { get; set; }

        [Required(ErrorMessage = "Team 2 Name is required")]
        [Display(Name = "Team 2 Name")]
        public string Team2Name { get; set; }

        public string Team1Score { get; set; }
        public string Team2Score { get; set; }
    }
}