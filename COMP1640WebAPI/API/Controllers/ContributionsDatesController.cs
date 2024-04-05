using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.DataAccess.Data;

namespace COMP1640WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsDateController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public ContributionsDateController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // POST: api/ContributionsDate
        [HttpPost]
        public async Task<ActionResult<ContributionsDates>> PostContributionsDate(ContributionsDates contributionsDate)
        {
            if (contributionsDate.endDate <= contributionsDate.startDate ||
                (contributionsDate.endDate - contributionsDate.startDate).Value.TotalDays <= 30 ||
                contributionsDate.finalEndDate <= contributionsDate.endDate ||
                (contributionsDate.finalEndDate - contributionsDate.endDate).Value.TotalDays <= 7)
            {
                return BadRequest("Invalid dates. End Date should be after Start Date and more than 1 month, Final End Date should be after End Date and more than 1 week.");
            }

            _context.ContributionsDates.Add(contributionsDate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContributionsDate", new { id = contributionsDate.contributionsDateId }, contributionsDate);
        }

        // GET: api/ContributionsDate
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContributionsDates>>> GetContributionsDate()
        {
            return await _context.ContributionsDates.ToListAsync();
        }

        // GET: api/ContributionsDate/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ContributionsDates>> GetContributionsDate(int id)
        {
            var contributionsDate = await _context.ContributionsDates.FindAsync(id);

            if (contributionsDate == null)
            {
                return NotFound();
            }

            return contributionsDate;
        }

        // PUT: api/ContributionsDate/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContributionsDate(int id, ContributionsDates contributionsDate)
        {
            if (id != contributionsDate.contributionsDateId)
            {
                return BadRequest();
            }

            _context.Entry(contributionsDate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContributionsDateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ContributionsDate/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContributionsDate(int id)
        {
            var contributionsDate = await _context.ContributionsDates.FindAsync(id);
            if (contributionsDate == null)
            {
                return NotFound();
            }

            _context.ContributionsDates.Remove(contributionsDate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContributionsDateExists(int id)
        {
            return _context.ContributionsDates.Any(e => e.contributionsDateId == id);
        }
    }
}
