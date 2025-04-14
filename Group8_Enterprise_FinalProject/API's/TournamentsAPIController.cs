using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Messages;

namespace Group8_Enterprise_FinalProject.API_s
{
    /// <summary>
    /// Controller class for the routes of the Tournaments API
    /// </summary>
    [ApiController]
    [Route("tournaments-api")]
    public class TournamentsAPIController : ControllerBase
    {
        private readonly TournamentDbContext _tournamentDbContext;
        public TournamentsAPIController(TournamentDbContext context)
        {
            _tournamentDbContext = context;
        }

        /// <summary>
        /// API home route – provides basic configuration data
        /// </summary>
        [HttpGet]
        public IActionResult GetTournamentsApiHome()
        {
            var apiHomeModel = new TournamentsApiHomeDTO()
            {
                Links = new Dictionary<string, Link>()
                {
                    { "self", new Link { Href = GenerateFullUrl("/tournaments-api"), Rel = "self", Method = "GET" } },
                    { "all-tournaments", new Link { Href = GenerateFullUrl("/tournaments-api/tournaments"), Rel = "tournaments", Method = "GET" } },
                    { "register", new Link { Href = GenerateFullUrl("/tournaments-api/tournaments/{tournamentId}/register"), Rel = "register", Method = "POST" } },
                    { "game-details", new Link { Href = GenerateFullUrl("/tournaments-api/games/{gameId}"), Rel = "game-details", Method = "GET" } },
                },
                ApiVersion = "1.0",
                Creator = "Group8"
            };
            return Ok(apiHomeModel);
        }

        // 1. Return all tournaments
        [HttpGet("tournaments")]
        public IActionResult GetAllTournaments()
        {
            var tournaments = _tournamentDbContext.Tournaments.ToList();
            if (tournaments == null || tournaments.Count == 0)
            {
                return NoContent();
            }
            return Ok(tournaments);
        }

        // 2. Return all games for a specific tournament
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
            return Ok(tournament.Games);
        }

        // 3. Submit registration details for a specific tournament
        [HttpPost("tournaments/{tournamentId:int}/register")]
        public IActionResult RegisterPlayer(int tournamentId, [FromBody] TournamentRegistrationDTO registrationDTO)
        {
            if (registrationDTO == null ||
                string.IsNullOrWhiteSpace(registrationDTO.Name) ||
                string.IsNullOrWhiteSpace(registrationDTO.Email))
            {
                return BadRequest("Name and email must be provided.");
            }

            var tournament = _tournamentDbContext.Tournaments.Find(tournamentId);
            if (tournament == null)
            {
                return NotFound($"Tournament with id {tournamentId} not found.");
            }

            // Create a new registration entity.
            var registration = new TournamentRegistration
            {
                Name = registrationDTO.Name,
                Email = registrationDTO.Email,
                TournamentId = tournamentId
            };

            _tournamentDbContext.TournamentRegistrations.Add(registration);
            _tournamentDbContext.SaveChanges();

            // Return 201 Created along with the location of the new registration
            return CreatedAtAction(nameof(RegisterPlayer),
                new { tournamentId = tournamentId, registrationId = registration.TournamentRegistrationId },
                registration);
        }

        // 4. Retrieve details for an individual game
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
            return Ok(game);
        }

        // Utility method to generate the full URL (used for creating HATEOAS links, if desired)
        private string GenerateFullUrl(string path)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}