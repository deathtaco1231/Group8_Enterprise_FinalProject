using System.ComponentModel.DataAnnotations;

namespace Group8_Enterprise_FinalProject.Entities
{
    public class TournamentRegistration
    {
        public int TournamentRegistrationId { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        // The tournament the player is registering for
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; } = null!;
    }
}