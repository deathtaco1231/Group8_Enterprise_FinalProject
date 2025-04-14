using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Models;
using Group8_Enterprise_FinalProject.Services;
using Group8_Enterprise_FinalProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Group8_Enterprise_FinalProject.Controllers
{
    public class GameController : Controller
    {
        private TournamentDbContext _tournamentDbContext;
        private ITournamentManagerService _tournamentManagerService;
        public GameController(TournamentDbContext tournamentDbContext, ITournamentManagerService tournamentManagerService)
        {
            _tournamentDbContext = tournamentDbContext;
            _tournamentManagerService = tournamentManagerService;
        }

        /// <summary>
        /// Returns view for managing a game to caller (could be any user since subsequent actions require authorization)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/Games/{id}")]
        public IActionResult GetManageForm(int id)
        {
            Game game = _tournamentDbContext.Games.Where(g => g.GameId == id).Include(g => g.Teams).ThenInclude(te => te.Players).Include(te => te.Tournament).FirstOrDefault();

            if (game == null)
            {
                return NotFound();
            }
            else
            {
                GameViewModel gameViewModel = new GameViewModel()
                {
                    ActiveGame = game,
                    WinningTeamName = game.GetWinningTeam() != null ? game.Teams.FirstOrDefault(t => t.TeamId == game.GetWinningTeam()).Name : "TBD"
                };

                return View("Manage", gameViewModel);
            }
        }

        /// <summary>
        /// Returns the view containing form to edit a game based on the ID passed as parameter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Games/Edit/{id}")]
        public IActionResult GetEditForm(int id)
        {
            Game game = _tournamentDbContext.Games.Where(g => g.GameId == id).Include(g => g.Teams).ThenInclude(te => te.Players).Include(te => te.Tournament).FirstOrDefault();
            if (game == null)
            {
                return NotFound();
            }
            else
            {
                GameViewModel gameViewModel = new GameViewModel()
                {
                    ActiveGame = game,
                    WinningTeamName = game.GetWinningTeam() != null ? game.Teams.FirstOrDefault(t => t.TeamId == game.GetWinningTeam()).Name : "TBD"
                };

                return View("Edit", gameViewModel);
            }
        }

        /// <summary>
        /// Returns confirmation form for organizer to ensure they wish to delete a game before going ahead with the deletion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Games/Delete/{id}")]
        public IActionResult GetDeleteForm(int id)
        {
            Game game = _tournamentDbContext.Games.Where(g => g.GameId == id).Include(g => g.Teams).ThenInclude(te => te.Players).Include(te => te.Tournament).FirstOrDefault();
            if (game == null)
            {
                return NotFound();
            }
            else
            {
                return View("Delete", game);
            }
        }

        /// <summary>
        /// Returns the view containing form to create a new game 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Tournaments/{id}/Games/Create")]
        public IActionResult GetCreateForm(int id)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Where(to => to.TournamentId == id).Include(to => to.Games).ThenInclude(ga => ga.Teams).FirstOrDefault();
            if (tournament == null)
            {
                return NotFound();
            }
            GameViewModel gameViewModel = new GameViewModel
            {
                ActiveGame = new Game(),
                WinningTeamName = "TBD"
            };


            gameViewModel.ActiveGame.GameDateTime = DateTime.Now;
            gameViewModel.ActiveGame.Tournament = tournament;
            gameViewModel.ActiveGame.TournamentId = tournament.TournamentId;

            return View("Create", gameViewModel);
        }

        /// <summary>
        /// Performs the creation of the new game, adding to database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gameViewModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpPost("/Tournaments/{id}/Games/Create")]
        public IActionResult CreateNewGame(int id, GameViewModel gameViewModel)
        {
            Tournament tournament = _tournamentDbContext.Tournaments.Where(to => to.TournamentId == id).Include(to => to.Games).ThenInclude(ga => ga.Teams).FirstOrDefault();
            if (tournament == null)
            {
                return NotFound();
            }

            gameViewModel.ActiveGame.Tournament = tournament;
            gameViewModel.ActiveGame.TournamentId = tournament.TournamentId;

            if (ModelState.IsValid)
            {
                _tournamentDbContext.Games.Add(gameViewModel.ActiveGame);
                _tournamentDbContext.Teams.AddRange(gameViewModel.ActiveGame.Teams);
                _tournamentDbContext.SaveChanges();

                return RedirectToAction("GetManageForm", "Tournament", new { id = tournament.TournamentId });
            }
            else
            {
                return View("Create", gameViewModel);
            }
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost("/Games/Edit/{id}")]
        public IActionResult HandleEditRequest(int id, GameViewModel gameViewModel)
        {
            Game game = _tournamentDbContext.Games.Where(g => g.GameId == id).Include(g => g.Teams).ThenInclude(te => te.Players).Include(te => te.Tournament).FirstOrDefault();
            if (game == null)
            {
                return NotFound();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    game.GameDateTime = gameViewModel.ActiveGame.GameDateTime;
                    _tournamentDbContext.SaveChanges();
                    return RedirectToAction("GetManageForm", "Tournaments", new { game.TournamentId });
                }
                else
                {
                    return View("Edit", gameViewModel);
                }
            }
        }

        /// <summary>
        /// Returns view with teams found inside game so that organizer can add more registered players to either/both team
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="teamId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpGet("/Game/{gameId}/Team/{teamId}/AssignPlayer")]
        public IActionResult AssignPlayer(int gameId, int teamId)
        {
            // Retrieve the game (with its associated Tournament)
            var game = _tournamentDbContext.Games
                        .Include(g => g.Tournament)
                        .FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return NotFound();
            }
            if (game.Tournament == null)
            {
                return BadRequest("Game is not associated with a tournament.");
            }

            // Get list of current player names in this game
            var currentPlayers = _tournamentDbContext.Players
                .Join(_tournamentDbContext.Teams,
                      p => p.TeamId,
                      t => t.TeamId,
                      (p, t) => new { p, t })
                .Where(pt => pt.t.GameId == gameId)
                .Select(pt => pt.p.Name)
                .Distinct()
                .ToList();

            // Get available tournament registrations for the same tournament
            // that are not already assigned (by checking if the registration’s Name is not among current players)
            var availableRegistrations = _tournamentDbContext.TournamentRegistrations
                .Where(tr => tr.TournamentId == game.Tournament.TournamentId &&
                             !currentPlayers.Contains(tr.Name))
                .ToList();

            var viewModel = new TeamPlayerAssignmentViewModel
            {
                GameId = gameId,
                TeamId = teamId,
                TournamentId = game.Tournament.TournamentId,
                AvailableRegistrations = availableRegistrations
            };

            return View("AssignPlayer", viewModel);
        }

        /// <summary>
        /// Performs the assignment of a player to a team, returns back to the manage form of the game that the user was originally on
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer")]
        [HttpPost("/Game/{gameId}/Team/{teamId}/AssignPlayer")]
        public IActionResult AssignPlayer(TeamPlayerAssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AssignPlayer", model);
            }

            // Check if the incoming team id is 0, indicating the team doesn't exist yet
            if (model.TeamId == 0)
            {
                var newTeam = new Team
                {
                    // Organizer can already specify this later so this is fine for now
                    Name = "New Team",
                    GameId = model.GameId,
                    TournamentId = model.TournamentId
                };

                _tournamentDbContext.Teams.Add(newTeam);
                _tournamentDbContext.SaveChanges();

                // Update model.TeamId with the newly created team id.
                model.TeamId = newTeam.TeamId;
            }

            // Now create the Player record using the valid team id.
            var player = new Player
            {
                Name = _tournamentDbContext.TournamentRegistrations
                            .Where(tr => tr.TournamentRegistrationId == model.SelectedRegistrationId)
                            .Select(tr => tr.Name)
                            .FirstOrDefault(),
                TeamId = model.TeamId
            };

            _tournamentDbContext.Players.Add(player);
            _tournamentDbContext.SaveChanges();

            Tournament? tmpTournament = _tournamentDbContext.Tournaments.Where(t => t.TournamentId == model.TournamentId).FirstOrDefault();
            Game? tmpGame = _tournamentDbContext.Games.Where(g => g.GameId == model.GameId).FirstOrDefault();
            string? email = _tournamentDbContext.TournamentRegistrations
                            .Where(tr => tr.TournamentRegistrationId == model.SelectedRegistrationId)
                            .Select(tr => tr.Email)
                            .FirstOrDefault();

            string emailBody = _tournamentManagerService.FormatReminderEmail(player.Name, tmpTournament.Name, model.GameId, tmpTournament.Game, 60);
            string subject = "Registration for " + tmpTournament.Name + " Tournament";

            _tournamentManagerService.SendPlayerEmailWithDelay(email, subject, emailBody, tmpGame.GameDateTime, -60);

            return RedirectToAction("GetManageForm", "Game", new { id = model.GameId });
        }
    }
}
