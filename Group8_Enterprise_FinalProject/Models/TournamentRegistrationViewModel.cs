using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Models
{
    public class TournamentRegistrationViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        // The tournamentId is provided via query or as a hidden field
        public int TournamentId { get; set; }
    }
}
