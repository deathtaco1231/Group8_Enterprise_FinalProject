using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Messages;
using Group8_Enterprise_FinalProject.Services;

namespace Group8_Enterprise_FinalProject.API_s
{
    /// <summary>
    /// Controller for handling tournament-related API requests 
    /// </summary>
    [ApiController]
    [Route("tournaments-api")]
    public class TournamentsAPIController : ControllerBase
    {
        private TournamentDbContext _tournamentDbContext;
        private ITournamentManagerService _tournamentManagerService;

        public TournamentsAPIController(TournamentDbContext context, ITournamentManagerService tournamentManagerService)
        {
            _tournamentDbContext = context;
            _tournamentManagerService = tournamentManagerService;
        }

        /// <summary>
        /// API home route, providing base configuration and endpoint references (similar to PA3)
        /// </summary>
        [HttpGet("")]
        public IActionResult GetApiHome()
        {
            var homeDto = new TournamentsApiHomeDTO
            {
                Links = new Dictionary<string, Link> {
                    { "self", new Link { Href = GenerateFullUrl("/tournaments-api"), Rel = "self", Method = "GET" } },
                    { "all-tournaments", new Link { Href = GenerateFullUrl("/tournaments-api/tournaments"), Rel = "tournaments", Method = "GET" } },
                    { "register", new Link { Href = GenerateFullUrl("/tournaments-api/tournaments/{tournamentId}/register"), Rel = "register", Method = "POST" } },
                    { "game-details", new Link { Href = GenerateFullUrl("/tournaments-api/games/{gameId}"), Rel = "game-details", Method = "GET" } },
                },
                ApiVersion = "1.0",
                Creator = "Group8"
            };
            return Ok(homeDto);
        }

        /// <summary>
        /// Return all tournaments to API caller
        /// </summary>
        [HttpGet("tournaments")]
        public IActionResult GetAllTournaments()
        {
            var tournaments = _tournamentDbContext.Tournaments.ToList();
            if (!tournaments.Any())
            {
                return NoContent();
            }

            var dtoList = tournaments.Select(t => new TournamentDetails
            {
                TournamentId = t.TournamentId,
                Name = t.Name ?? string.Empty,
                Game = t.Game ?? string.Empty,
                NumPlayersPerTeam = t.NumPlayersPerTeam,
                StartDateTime = t.StartDateTime,
                NumGames = t.NumGames,
                Url = GenerateFullUrl($"/tournaments-api/tournaments/{t.TournamentId}")
            }).ToList();

            return Ok(new { tournaments = dtoList });
        }

        /// <summary>
        /// Return all games for a specific tournament by the tournaments ID value passed as parameterk
        /// </summary>
        [HttpGet("tournaments/{tournamentId:int}/games")]
        public IActionResult GetGamesForTournament(int tournamentId)
        {
            var tournament = _tournamentDbContext.Tournaments
                .Include(t => t.Games)
                    .ThenInclude(g => g.Teams)
                .FirstOrDefault(t => t.TournamentId == tournamentId);
            if (tournament == null)
            {
                return NotFound($"Tournament with id {tournamentId} not found.");
            }

            var gamesDto = tournament.Games.Select(g => new GameDetails
            {
                GameId = g.GameId,
                GameDateTime = g.GameDateTime,
                Result = g.Result,
                // Using the team name (or a fallback if not set) for every team in the game.
                TeamNames = g.Teams.Select(team => team.Name ?? "Unnamed Team").ToList(),
                Url = GenerateFullUrl($"/tournaments-api/games/{g.GameId}")
            }).ToList();

            return Ok(new { games = gamesDto });
        }

        /// <summary>
        /// Submit registration details (name and email) for a specific tournament
        /// </summary>
        [HttpPost("tournaments/{tournamentId:int}/register")]
        public IActionResult RegisterPlayer(int tournamentId, [FromBody] TournamentRegistrationDTO regDto)
        {
            if (regDto == null || string.IsNullOrWhiteSpace(regDto.Name) || string.IsNullOrWhiteSpace(regDto.Email) || !(regDto.Email.Contains('@') && regDto.Email.Contains('.')))
            {
                return BadRequest("Name and valid email are required.");
            }

            var tournament = _tournamentDbContext.Tournaments.Find(tournamentId);
            if (tournament == null)
            {
                return NotFound($"Tournament with id {tournamentId} not found.");
            }

            var registration = new TournamentRegistration
            {
                Name = regDto.Name,
                Email = regDto.Email,
                TournamentId = tournamentId
            };

            _tournamentDbContext.TournamentRegistrations.Add(registration);
            _tournamentDbContext.SaveChanges();

            // Send player their registration confirmation email once registration has been added to the database successfully
            string emailBody = _tournamentManagerService.FormatRegistrationEmail(regDto.Name, tournament.Name, tournament.TournamentId, tournament.Game);
            string subject = "Registration for " + tournament.Name + " Tournament";
            string email = regDto.Email;
            _tournamentManagerService.SendPlayerEmail(email, subject, emailBody);

            var resultDto = new TournamentRegistrationDetails
            {
                TournamentRegistrationId = registration.TournamentRegistrationId,
                Name = registration.Name,
                Email = registration.Email,
                Url = GenerateFullUrl($"/tournaments-api/tournaments/{tournamentId}/registrations/{registration.TournamentRegistrationId}")
            };

            return CreatedAtAction(nameof(RegisterPlayer),
                new { tournamentId = tournamentId, registrationId = registration.TournamentRegistrationId },
                resultDto);
        }

        /// <summary>
        /// Retrieve details for an individual game by ID passed as parameter
        /// </summary>
        [HttpGet("games/{gameId:int}")]
        public IActionResult GetGameById(int gameId)
        {
            var game = _tournamentDbContext.Games
                .Include(g => g.Teams)
                .Include(g => g.Tournament)
                .FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return NotFound();
            }

            var dto = new GameDetails
            {
                GameId = game.GameId,
                GameDateTime = game.GameDateTime,
                Result = game.Result,
                TeamNames = game.Teams.Select(team => team.Name ?? "Unnamed Team").ToList(),
                Url = GenerateFullUrl($"/tournaments-api/games/{game.GameId}")
            };

            return Ok(dto);
        }

        /// <summary>
        /// Utility method: generates a fully qualified URL based on the given path.
        /// </summary>
        private string GenerateFullUrl(string path)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}