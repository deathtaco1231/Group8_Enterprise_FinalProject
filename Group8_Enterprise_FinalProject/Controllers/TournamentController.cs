using Microsoft.AspNetCore.Mvc;
using Group8_Enterprise_FinalProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Group8_Enterprise_FinalProject.Models;
using Group8_Enterprise_FinalProject.Services;
using Microsoft.AspNetCore.Authorization;

namespace Group8_Enterprise_FinalProject.Controllers
{
    /// <summary>
    /// Controller defining actions for tournament management (create, edit, etc.)
    /// </summary>
    public class TournamentController : Controller
    {
        private TournamentDbContext _tournamentDbContext;
        private ITournamentManagerService _tournamentManagerService;
        public TournamentController(TournamentDbContext tournamentDbContext, ITournamentManagerService tournamentManagerService)
        {
            _tournamentDbContext = tournamentDbContext;
            _tournamentManagerService = tournamentManagerService;
        }

        /// <summary>
        /// Simple list of tournaments returned from database (if any exist)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/Tournaments")]
        public IActionResult GetAllTournaments()
        {
            List<Tournament> tournaments = _tournamentDbContext.Tournaments.Include(to => to.Games).ThenInclude(ga => ga.Teams).ToList();
            if (tournaments == null || tournaments.Count == 0)
            {
                return NoContent();
            }

            return View("List", tournaments);
        }

        /// <summary>
        /// Returns view with details of tournament. Game details are accesed through this for each tournament.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Tournaments/{id}")]
        public IActionResult GetManageForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Where(to => to.TournamentId == id).Include(to => to.Games).ThenInclude(ga => ga.Teams).First();
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

        /// <summary>
        /// Returns view with empty model for creating a brand new tournament.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Tournaments/Create")]
        public IActionResult GetCreateForm()
        {
            TournamentViewModel tournamentViewModel = new TournamentViewModel
            {
                ActiveTournament = new Tournament(),
            };

            tournamentViewModel.ActiveTournament.StartDateTime = DateTime.Now;

            return View("Create", tournamentViewModel);
        }

        // GET: Edit Tournament
        /// <summary>
        /// Returns view with tournament details for editing. The tournament is passed as a parameter to the view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Tournaments/Edit/{id}")]
        public IActionResult GetEditForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).FirstOrDefault(to => to.TournamentId == id);
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

                return View("Edit", tournamentViewModel);
            }
        }

        /// <summary>
        /// Returns view with tournament details for deleting. The tournament is passed as a parameter to the view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
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

        /// <summary>
        /// Handles a creation request after user submits (completed) creation form, or rejects it if invalid.
        /// </summary>
        /// <param name="tournamentViewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
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

        /// <summary>
        /// Handles an edit request after user submits (completed) edit form, or rejects it if invalid.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tournamentViewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
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

        /// <summary>
        /// Deletes the tournament of ID value passed as parameter (if it exists within the database)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpPost("/Tournaments/Delete/{id}")]
        public IActionResult HandleDeleteRequest(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Include(to => to.Games).ThenInclude(to => to.Teams).FirstOrDefault(to => to.TournamentId == id);
            if (tournament == null)
            {
                return NotFound();
            }
            else
            {
                // Deleting dependent teams first (forgot to set up cascade delete in DB so this takes place of that)
                if (tournament.Teams != null && tournament.Teams.Any())
                {
                    _tournamentDbContext.Teams.RemoveRange(tournament.Teams);
                }
                
                _tournamentDbContext.Tournaments.Remove(tournament);
                _tournamentDbContext.SaveChanges();

                return RedirectToAction("GetAllTournaments");
            }
        }

        /// <summary>
        /// Returns the Registration form for a tournament of ID value passed as parameter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Tournaments/{id}/Register")]
        public IActionResult Register(int id)
        {
            // 'id' is the TournamentId.
            var viewModel = new TournamentRegistrationViewModel
            {
                TournamentId = id
            };
            return View(viewModel);
        }

        /// <summary>
        /// Accepts and checks the registration form from the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/Tournaments/{id}/Register")]
        public IActionResult Register(TournamentRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map the view model to the domain entity
            var registration = new TournamentRegistration
            {
                Name = model.Name,
                Email = model.Email,
                TournamentId = model.TournamentId
            };

            _tournamentDbContext.TournamentRegistrations.Add(registration);
            _tournamentDbContext.SaveChanges();

            Tournament? tmpTournament = _tournamentDbContext.Tournaments.Where(to => to.TournamentId == model.TournamentId).FirstOrDefault();

            string emailBody = _tournamentManagerService.FormatRegistrationEmail(model.Name, tmpTournament.Name, model.TournamentId, tmpTournament.Game);
            string subject = "Registration for " + tmpTournament.Name + " Tournament";
            string email = model.Email;
            _tournamentManagerService.SendPlayerEmail(email, subject, emailBody);

            return Redirect($"/Tournaments/{model.TournamentId}");
        }

    }
}
