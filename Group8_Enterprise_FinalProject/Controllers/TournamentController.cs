using Microsoft.AspNetCore.Mvc;
using Group8_Enterprise_FinalProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Group8_Enterprise_FinalProject.Models;

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

        // GET: Tournament Details
        [HttpGet("/Tournaments/{id}")]
        public IActionResult GetManageForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Where(to => to.TournamentId == id).Include(to => to.Games).First();
            if (tournament == null)
            {
                return NotFound();
            }
            else
            {
                TournamentViewModel tournamentViewModel = new TournamentViewModel()
                {
                    ActiveTournament = tournament
                };

                return View("Manage", tournamentViewModel);
            }
        }

        // GET: Create Tournament
        [HttpGet("/Tournaments/Create")]
        public IActionResult GetCreateForm()
        {
            TournamentViewModel tournamentViewModel = new TournamentViewModel
            {
                ActiveTournament = new Tournament(),
            };

            return View("Create", tournamentViewModel);
        }

        // GET: Edit Tournament
        [HttpGet("/Tournaments/Edit/{id}")]
        public IActionResult GetEditForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).FirstOrDefault(to => to.TournamentId == id);
            if (tournament == null)
            {
                return NotFound();
            }
            return View("Edit", tournament);
        }

        // GET: Delete Tournament
        [HttpGet("/Tournaments/Delete/{id}")]
        public IActionResult GetDeleteForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).FirstOrDefault(to => to.TournamentId == id);
            if (tournament == null)
            {
                return NotFound();
            }
            return View("Delete", tournament);
        }

        // POST: Create Tournament
        [HttpPost("/Tournaments/Create")]
        public IActionResult CreateNewTournament(TournamentViewModel tournamentViewModel)
        {
            if (ModelState.IsValid)
            {
                _tournamentDbContext.Tournaments.Add(tournamentViewModel.ActiveTournament);
                _tournamentDbContext.SaveChanges();

                int newTournamentId = tournamentViewModel.ActiveTournament.TournamentId;

                return RedirectToAction("GetManageForm", "Tournament", new { id = newTournamentId });
            }
            return View("Create", tournamentViewModel);
        }

        // POST: Edit Tournament
        [HttpPost("/Tournaments/Edit/{id}")]
        public IActionResult HandleEditRequest(int id, TournamentViewModel tournamentViewModel)
        {
            if (ModelState.IsValid)
            {
                Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).FirstOrDefault(to => to.TournamentId == id);
                if (tournament == null)
                {
                    return NotFound();
                }
                else
                {
                    tournament.Name = tournamentViewModel.ActiveTournament.Name;
                    tournament.Game = tournamentViewModel.ActiveTournament.Game;
                    tournament.NumPlayersPerTeam = tournamentViewModel.ActiveTournament.NumPlayersPerTeam;
                    tournament.StartDateTime = tournamentViewModel.ActiveTournament.StartDateTime;
                    tournament.NumGames = tournamentViewModel.ActiveTournament.NumGames;
                    _tournamentDbContext.SaveChanges();
                    return RedirectToAction("GetManageForm", "Tournament", new { id });
                }
            }
            else
            {
                return View("Edit", tournamentViewModel);
            }
        }

        // POST: Delete Tournament
        [HttpPost("/Tournaments/Delete/{id}")]
        public IActionResult HandleDeleteRequest(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).FirstOrDefault(to => to.TournamentId == id);
            if (tournament == null)
            {
                return NotFound();
            }
            else
            {
                _tournamentDbContext.Tournaments.Remove(tournament);
                _tournamentDbContext.SaveChanges();
                return RedirectToAction("GetAllTournaments");
            }
        }

        private readonly TournamentDbContext _tournamentDbContext;

    }
}
