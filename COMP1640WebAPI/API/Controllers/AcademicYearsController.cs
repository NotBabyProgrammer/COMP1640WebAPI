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
    public class AcademicYearsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public AcademicYearsController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/AcademicYears
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicYears>>> GetAcademicYears()
        {
            return await _context.AcademicYears.ToListAsync();
        }

        // GET: api/AcademicYears/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicYears>> GetAcademicYear(int id)
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);

            if (academicYear == null)
            {
                return NotFound();
            }

            return academicYear;
        }

        // PUT: api/AcademicYears/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{academicYearsId}")]
        public async Task<IActionResult> PutAcademicYear(int academicYearsId, AcademicYears academicYear)
        {
            if (academicYearsId != academicYear.academicYearsId)
            {
                return BadRequest();
            }

            _context.Entry(academicYear).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademicYearExists(academicYearsId))
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

        // POST: api/AcademicYears
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AcademicYears>> PostAcademicYear(AcademicYears academicYear)
        {
            if (!academicYear.startDays.HasValue || !academicYear.endDays.HasValue || !academicYear.finalEndDays.HasValue)
            {
                return BadRequest("Start date, end date, and final end date are required.");
            }

            var startDate = new DateTime(academicYear.startDays.Value.Year, academicYear.startDays.Value.Month, academicYear.startDays.Value.Day);
            var endDate = new DateTime(academicYear.endDays.Value.Year, academicYear.endDays.Value.Month, academicYear.endDays.Value.Day);
            var finalEndDate = new DateTime(academicYear.finalEndDays.Value.Year, academicYear.finalEndDays.Value.Month, academicYear.finalEndDays.Value.Day);

            var diff1 = (endDate - startDate).TotalDays;
            var diff2 = (finalEndDate - endDate).TotalDays;

            if (diff1 < 30)
            {
                return BadRequest("End date must be 1 month or more after the start date.");
            }

            if (diff2 < 7)
            {
                return BadRequest("Final end date must be 1 week or more after the end date.");
            }

            _context.AcademicYears.Add(academicYear);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcademicYear", new { id = academicYear.academicYearsId, startDays = academicYear.startDays, endDays = academicYear.endDays, finalEndDays = academicYear.finalEndDays }, academicYear);
        }

        // DELETE: api/AcademicYears/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcademicYear(int id)
        {
            var academicYear = await _context.AcademicYears.FindAsync(id);
            if (academicYear == null)
            {
                return NotFound();
            }

            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AcademicYearExists(int id)
        {
            return _context.AcademicYears.Any(e => e.academicYearsId == id);
        }
    }
}
