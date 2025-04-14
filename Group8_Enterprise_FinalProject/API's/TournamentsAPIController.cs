using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Group8_Enterprise_FinalProject.Entities;
using Group8_Enterprise_FinalProject.Messages;


namespace Group8_Enterprise_FinalProject.API_s
{
    /// <summary>
    /// Controller class for the routes of the Tournaments API managed by this ASP.NET Core app
    /// </summary>
    [ApiController]
    [Route("tournaments-api")]
    public class TournamentsAPIController : ControllerBase
    {
        private readonly TournamentDbContext _tournamentDbContext;
        public TournamentsAPIController(TournamentDbContext quotesDbContext)
        {
            _tournamentDbContext = quotesDbContext;
        }

        /// <summary>
        /// Similar to Task API example, essentially the basic info the SPA app needs to configure first at startup
        /// </summary>
        /// <returns></returns>
        [HttpGet("/")]
        public IActionResult GetTournamentsApiHome()
        {
            // Use quotes api home Data Transfer Object for this specific purpose
            var apiHomeModel = new TournamentsApiHomeDTO()
            {
                Links = new Dictionary<string, Link>()
                {
                    { "self", new Link { Href = GenerateFullUrl("/tournaments-api"), Rel = "self", Method = "GET" } },
                    { "tournaments", new Link { Href = GenerateFullUrl("/tournaments-api/tournaments"), Rel = "tournaments", Method = "GET" } },
                    { "games", new Link { Href = GenerateFullUrl("/tournaments-api/games"), Rel = "games", Method = "GET" } },            
                },
                ApiVersion = "1.0", // This and Creator dont do anything functionally meaningful in this app, but
                Creator = "Group8" // I included it anyway for the sake of future adaptability (could be used in future if desired)
            };
            return Ok(apiHomeModel);
        }

        /// <summary>
        /// Route for using API to retrieve list of all tournaments held by the app's database
        /// </summary>
        /// <returns></returns>
        [HttpGet("/tournaments")]
        public IActionResult GetAllTournaments()
        {
            var quotes = _tournamentDbContext.Tournaments
                .Include(q => q.Teams)
                .ThenInclude(ta => ta.TeamId)
                .ToList();

            if (quotes == null || quotes.Count == 0) // Fallback to safe return of no content if no quotes exist
            {
                return NoContent();
            }

            // Populate quote details object with contents of quote and URL for that quote's get by ID route for each quote
            // (sorted here since this is what impacts the display order, NOT the previous sort)
            var quotesDetailsObj = quotes.Select(q => new QuoteDetails
            {
                QuoteID = q.QuoteID,
                Content = q.Content,
                Author = q.Author,
                Likes = q.Likes,
                Tags = q.TagAssignments.Select(ta => ta.Tag.TagName).ToList(),
                Url = GenerateFullUrl($"/quotes/{q.QuoteID}")
            }).ToList(); // Resolve to list of quote details objects

            DateTime? lastModified; // Placing last modified obj here for greater scope (instead of return calls inside both the if and else blocks)
            if (quotes.Max(q => q.LastModified) == null)
            {
                lastModified = new DateTime(1970, 1, 1); // Safe default if no quotes exist (therefore no datetimes to get a max from)
            }
            else
            {
                // Since lastModified is nullable, this doesnt complain like it would if it wasn't nullable
                lastModified = quotes.Max(q => q.LastModified);
            }

            return Ok(new { quotes = quotesDetailsObj, quotesLastModified = lastModified });
        }

        private string GenerateFullUrl(string path) // Taken from Tasks example, makes it easy to get URL's in correct form (without weird extra / issue at end of URLs that I was getting before)
        {
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}
