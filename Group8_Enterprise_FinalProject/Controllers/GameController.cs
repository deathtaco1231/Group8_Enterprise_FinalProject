using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Group8_Enterprise_FinalProject.Controllers
{
    public class GameController : Controller
    {
        public GameController(TournamentDbContext tournamentDbContext)
        {
            _tournamentDbContext = tournamentDbContext;
        }

        // GET: Game By ID
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

        // GET: Edit Game
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

        // GET: Delete Game
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

        // GET: Create Game
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

            gameViewModel.ActiveGame.Tournament = tournament;
            gameViewModel.ActiveGame.TournamentId = tournament.TournamentId;

            return View("Create", gameViewModel);
        }

        // POST: Create Game
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

        private readonly TournamentDbContext _tournamentDbContext;
    }
}
