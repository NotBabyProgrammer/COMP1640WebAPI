using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public ContributionsController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Contributions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetContributions()
        {
            return await _context.Contributions.ToListAsync();
        }

        // GET: api/Contributions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contributions>> GetContributions(int id)
        {
            var contributions = await _context.Contributions.FindAsync(id);

            if (contributions == null)
            {
                return NotFound();
            }

            return contributions;
        }

        // PUT: api/Contributions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContributions(int id, Contributions contributions)
        {
            if (id != contributions.contributionId)
            {
                return BadRequest();
            }

            _context.Entry(contributions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContributionsExists(id))
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

        // POST: api/Contributions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contributions>> PostContributions(Contributions contributions)
        {
            _context.Contributions.Add(contributions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContributions", new { id = contributions.contributionId }, contributions);
        }

        // DELETE: api/Contributions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContributions(int id)
        {
            var contributions = await _context.Contributions.FindAsync(id);
            if (contributions == null)
            {
                return NotFound();
            }

            _context.Contributions.Remove(contributions);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContributionsExists(int id)
        {
            return _context.Contributions.Any(e => e.contributionId == id);
        }
    }
}
