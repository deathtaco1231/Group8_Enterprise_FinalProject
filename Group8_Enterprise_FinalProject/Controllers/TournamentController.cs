using Microsoft.AspNetCore.Mvc;
using Group8_Enterprise_FinalProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Group8_Enterprise_FinalProject.Controllers
{
    public class TournamentController : Controller
    {
        public TournamentController(TournamentDbContext tournamentDbContext)
        {
            _tournamentDbContext = tournamentDbContext;
        }

        // GET: All Tournaments
        [HttpGet("/Tournaments")]
        public IActionResult GetAllTournaments()
        {
            List<Tournament> tournaments = _tournamentDbContext.Tournaments.Include(to => to.Games).ToList();

            return View("List", tournaments);
        }

        private readonly TournamentDbContext _tournamentDbContext;

    }
}
