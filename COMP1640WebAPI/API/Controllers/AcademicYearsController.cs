﻿using System;
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
        public async Task<ActionResult<IEnumerable<AcademicYear>>> GetAcademicYears()
        {
            return await _context.AcademicYears.ToListAsync();
        }

        // GET: api/AcademicYears/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicYear>> GetAcademicYear(int id)
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicYear(int id, AcademicYear academicYear)
        {
            if (id != academicYear.Id)
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
                if (!AcademicYearExists(id))
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
        public async Task<ActionResult<AcademicYear>> PostAcademicYear(AcademicYear academicYear)
        {
            if ((academicYear.endYear - academicYear.startYear).Value.TotalDays > 11 * 30)
            {
                return BadRequest("End date must not be more than 11 months after the start date.");
            }

            _context.AcademicYears.Add(academicYear);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcademicYear", new { id = academicYear.Id }, academicYear);
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
            return _context.AcademicYears.Any(e => e.Id == id);
        }
    }
}
