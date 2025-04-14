using System;
using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Models
{
    public class CreateGameViewModel
    {
        [Required]
        [Display(Name = "Game Date & Time")]
        public DateTime GameDateTime { get; set; }

        [Required]
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "Team 1 Name is required")]
        [Display(Name = "Team 1 Name")]
        public string Team1Name { get; set; }

        [Required(ErrorMessage = "Team 2 Name is required")]
        [Display(Name = "Team 2 Name")]
        public string Team2Name { get; set; }
    }
}