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
    public class UsersController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public UsersController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            // Check if the specified roleId exists in the Roles table
            if (!_context.Roles.Any(r => r.roleId == users.roleId))
            {
                return BadRequest("Invalid roleId. Role does not exist.");
            }

            if (id != users.userId)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
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

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.userId == id);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult<Users>> Login(Users user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.userName == user.userName && u.password == user.password);

            if (existingUser == null)
            {
                return NotFound("Invalid username or password.");
            }

            return Ok("RoleId: " + existingUser.roleId);
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<Users>> Register(Users user)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.userName == user.userName))
            {
                return Conflict("Username already exists.");
            }

            // Check if the specified roleId exists in the Roles table
            if (!_context.Roles.Any(r => r.roleId == user.roleId))
            {
                return BadRequest("Invalid roleId. Role does not exist.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.userId }, user);
        }
    }
}
