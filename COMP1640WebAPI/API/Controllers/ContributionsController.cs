using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ContributionsController(COMP1640WebAPIContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // Other existing actions...

        // POST: api/Contributions/AddImage
        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image uploaded.");
            }

            try
            {
                // Save image to the server
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Add image path to database or do whatever is needed
                // Example: Save image path to Contributions table
                // Replace this with your actual logic to save the image path to the database
                var contributions = new Contributions
                {
                    // cai deo gi the nay ??? (BE Cong Linh)
                    imagePath = uniqueFileName  // Assuming you have a property named ImagePath in Contributions model
                };
                _context.Contributions.Add(contributions);
                await _context.SaveChangesAsync();

                return Ok("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Contributions/AddFile
        [HttpPost("AddFile")]
        public async Task<IActionResult> AddFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // Save file to the server
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "files");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Add file path to database or do whatever is needed
                // Example: Save file path to Contributions table
                // Replace this with your actual logic to save the file path to the database
                var contributions = new Contributions
                {
                    // cai deo gi the nay ??? (BE Cong Linh)
                    filePath = uniqueFileName  // Assuming you have a property named FilePath in Contributions model
                };
                _context.Contributions.Add(contributions);
                await _context.SaveChangesAsync();

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}