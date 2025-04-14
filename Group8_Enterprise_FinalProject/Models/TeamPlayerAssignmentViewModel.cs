using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Group8_Enterprise_FinalProject.Entities;

namespace Group8_Enterprise_FinalProject.Models
{
    public class TeamPlayerAssignmentViewModel
    {
        public int GameId { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "Please select a registration to assign.")]
        public int? SelectedRegistrationId { get; set; }

        // The list of registered players available for assignment (from TournamentRegistration table)
        public List<TournamentRegistration> AvailableRegistrations { get; set; } = new List<TournamentRegistration>();
    }
}