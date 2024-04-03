using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.BusinesLogic.DTO;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoordinatorsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public CoordinatorsController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Coordinators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coordinators>>> GetCoordinators()
        {
            return await _context.Coordinators.ToListAsync();
        }

        // GET: api/Coordinators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coordinators>> GetCoordinators(int id)
        {
            var coordinators = await _context.Coordinators.FindAsync(id);

            if (coordinators == null)
            {
                return NotFound();
            }

            return coordinators;
        }

        // PUT: api/Coordinators/5
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutCoordinators(int userId, CoordinatorsDTOPut coordinatorsDTO)
        {
            try
            {
                // Find the coordinator by userId
                var coordinators = await _context.Coordinators.FirstOrDefaultAsync(c => c.userId == userId);
                if (coordinators == null)
                {
                    return NotFound();
                }

                // Check if the facultyId exists in the Faculty table
                if (!_context.Faculties.Any(f => f.facultyId == coordinatorsDTO.facultyId))
                {
                    return NotFound("Faculty ID does not exist.");
                }

                // Update the facultyId if provided in the DTO
                coordinators.facultyId = coordinatorsDTO.facultyId;

                _context.Entry(coordinators).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/Coordinators
        [HttpPost]
        public async Task<ActionResult<Coordinators>> PostCoordinators(CoordinatorsDTOPost coordinators)
        {
            try
            {
                // Check if the user ID already exists in the Coordinators table
                if (_context.Coordinators.Any(c => c.userId == coordinators.userId))
                {
                    return Conflict("User ID already exists.");
                }

                // Check if the user ID has role ID 2 (assuming role ID 2 corresponds to the coordinator role)
                var user = await _context.Users.FindAsync(coordinators.userId);
                if (user == null || user.roleId != 2)
                {
                    return BadRequest("User does not have the Coordinator role.");
                }

                // Check if the faculty ID exists in the Faculty table
                if (!_context.Faculties.Any(f => f.facultyId == coordinators.facultyId))
                {
                    return NotFound("Faculty ID does not exist.");
                }

                // Create a new Coordinator instance
                var newCoordinator = new Coordinators
                {
                    userId = coordinators.userId,
                    facultyId = coordinators.facultyId
                };

                // Add the new coordinator to the database
                _context.Coordinators.Add(newCoordinator);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCoordinators", new { id = newCoordinator.coordinatorId }, newCoordinator);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // DELETE: api/Coordinators/5
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteCoordinators(int userId)
        {
            // Find the coordinator by userId
            var coordinators = await _context.Coordinators.FirstOrDefaultAsync(c => c.userId == userId);

            if (coordinators == null)
            {
                return NotFound();
            }

            _context.Coordinators.Remove(coordinators);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoordinatorsExists(int id)
        {
            return _context.Coordinators.Any(e => e.coordinatorId == id);
        }
    }
}
