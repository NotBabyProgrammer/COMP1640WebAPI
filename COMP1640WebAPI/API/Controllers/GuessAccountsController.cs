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
    public class GuessAccountsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public GuessAccountsController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/GuessAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuessAccounts>>> GetGuessAccounts()
        {
            return await _context.GuessAccounts.ToListAsync();
        }

        // GET: api/GuessAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GuessAccounts>> GetGuessAccounts(int id)
        {
            var guessAccounts = await _context.GuessAccounts.FindAsync(id);

            if (guessAccounts == null)
            {
                return NotFound();
            }

            return guessAccounts;
        }

        // PUT: api/GuessAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGuessAccounts(int id, GuessAccounts guessAccounts)
        {
            // Check if the specified facultyId exists in the Roles table
            if (!_context.Faculties.Any(f => f.facultyId == guessAccounts.facultyId))
            {
                return BadRequest("Invalid facultyId. Faculty does not exist.");
            }

            if (id != guessAccounts.guestId)
            {
                return BadRequest();
            }

            _context.Entry(guessAccounts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuessAccountsExists(id))
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

        // POST: api/GuessAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GuessAccounts>> PostGuessAccounts(GuessAccounts guessAccounts)
        {
            // Check if the specified roleId exists in the Roles table
            if (!_context.Faculties.Any(f => f.facultyId == guessAccounts.facultyId))
            {
                return BadRequest("Invalid facultyId. Faculty does not exist.");
            }

            _context.GuessAccounts.Add(guessAccounts);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGuessAccounts", new { id = guessAccounts.guestId }, guessAccounts);
        }

        // DELETE: api/GuessAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuessAccounts(int id)
        {
            var guessAccounts = await _context.GuessAccounts.FindAsync(id);
            if (guessAccounts == null)
            {
                return NotFound();
            }

            _context.GuessAccounts.Remove(guessAccounts);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuessAccountsExists(int id)
        {
            return _context.GuessAccounts.Any(e => e.guestId == id);
        }
    }
}
