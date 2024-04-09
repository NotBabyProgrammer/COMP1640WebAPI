using Microsoft.AspNetCore.Mvc;
using System.Linq;
using COMP1640WebAPI.DataAccess;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.DataAccess.Data;

namespace COMP1640WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public StatisticsController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Statistics/ContributionsPerFaculty
        [HttpGet("ContributionsPerFaculty")]
        public IActionResult GetContributionsPerFaculty()
        {
            var contributionsPerFaculty = _context.Contributions
                .GroupBy(c => c.facultyName)
                .Select(g => new
                {
                    facultyName = g.Key,
                    totalContributions = g.Count()
                })
                .ToList();

            return Ok(contributionsPerFaculty);
        }
        // GET: api/Statistics/PercentageOfContributionsByFaculty
        [HttpGet("PercentageOfContributionsByFaculty")]
        public IActionResult GetPercentageOfContributionsByFaculty()
        {
            var totalContributions = _context.Contributions.Count();

            var contributionsByFaculty = _context.Contributions
                .GroupBy(c => c.facultyName)
                .Select(g => new
                {
                    facultyName = g.Key,
                    Percentage = ((double)g.Count() / totalContributions) * 100
                })
                .ToList();

            return Ok(contributionsByFaculty);
        }

        // GET: api/Statistics/ContributionsWithoutComment
        [HttpGet("ContributionsWithoutComment")]
        public IActionResult GetContributionsWithoutComment()
        {
            var contributionsWithoutComment = _context.Contributions
                .Where(c => string.IsNullOrEmpty(c.comments))
                .ToList();

            return Ok(contributionsWithoutComment);
        }
    }
}
