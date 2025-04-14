using Group8_Enterprise_FinalProject.Entities;

namespace Group8_Enterprise_FinalProject.Models
{
    public class TournamentViewModel
    {
        public Tournament? ActiveTournament { get; set; } = new Tournament
        {
            StartDateTime = DateTime.Now
        };
    }
}
