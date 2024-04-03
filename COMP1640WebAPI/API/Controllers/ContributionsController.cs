using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using Microsoft.AspNetCore.StaticFiles;
using COMP1640WebAPI.BusinesLogic.DTO;
using AutoMapper;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;
        private readonly IMapper _mapper;

        public ContributionsController(COMP1640WebAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
        // PUT: api/Contributions/Review/5
        [HttpPut("Review/{id}")]
        public async Task<IActionResult> ReviewContributions(int id, [FromForm] ContributionsDTOReview contributionsDTO)
        {
            var contributions = await _context.Contributions.FindAsync(id);
            if (contributions == null)
            {
                return NotFound();
            }
            // Update properties if provided in the DTO
            contributions.approval = contributionsDTO.approval.Value;
            contributions.comments = contributionsDTO.comments;

            _context.Entry(contributions).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PUT: api/Contributions/Edit/5
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditContributions(int id, [FromForm] ContributionsDTOEdit contributionsDTO, List<IFormFile> files, List<IFormFile> images)
        {
            var contributions = await _context.Contributions.FindAsync(id);
            List<string> filePaths = new List<string>();
            List<string> imagePaths = new List<string>();

            if (files == null || files.Count == 0)
            {
                return BadRequest("Files not provided.");
            }
            if (images == null || images.Count == 0)
            {
                return BadRequest("Images not provided.");
            }
            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return BadRequest("File is empty.");
                }
                filePaths.Add(await WriteFile(file, "Files"));
            }

            foreach (var image in images)
            {
                if (image.Length == 0)
                {
                    return BadRequest("Image is empty.");
                }
                imagePaths.Add(await WriteFile(image, "Images"));
            }
            if (contributionsDTO.title != null)
            {
                contributions.title = contributionsDTO.title;
            }
            contributions.filePaths = filePaths;
            contributions.imagePaths = imagePaths;
            _context.Entry(contributions).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Contributions
        [HttpPost("AddArticles")]
        public async Task<ActionResult<Contributions>> PostContributions([FromForm] ContributionsDTOPost contributionsDTO, List<IFormFile> files, List<IFormFile> images, CancellationToken cancellationToken)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest("Files not provided.");
                }
                if (images == null || images.Count == 0)
                {
                    return BadRequest("Images not provided.");
                }

                List<string> filePaths = new List<string>();
                List<string> imagePaths = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        return BadRequest("File is empty.");
                    }
                    filePaths.Add(await WriteFile(file, "Files"));
                }

                foreach (var image in images)
                {
                    if (image.Length == 0)
                    {
                        return BadRequest("Image is empty.");
                    }
                    imagePaths.Add(await WriteFile(image, "Images"));
                }

                var contributions = new Contributions
                {
                    userId = contributionsDTO.userId,
                    title = contributionsDTO.title,
                    filePaths = filePaths,
                    imagePaths = imagePaths,
                    submissionDate = DateTime.Now,
                    closureDate = DateTime.Now.AddDays(14),
                    status = "on-time",
                    approval = false,
                    facultyName = contributionsDTO.facultyName
                };

                _context.Contributions.Add(contributions);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetContributions", new { id = contributions.contributionId }, contributions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<string> WriteFile(IFormFile file, string folderName)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = DateTime.Now.Ticks.ToString() + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\{folderName}");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\{folderName}", filename);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
            }
            return filename;
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
        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            try
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "API\\Upload\\Files", filename);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filepath, out var contenttype))
                {
                    contenttype = "application/octet-stream";
                }

                var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                return File(bytes, contenttype, Path.GetFileName(filepath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        [Route("DownloadImage")]
        public async Task<IActionResult> DownloadImage(string filename)
        {
            try
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "API\\Upload\\Images", filename);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filepath, out var contenttype))
                {
                    contenttype = "application/octet-stream";
                }

                var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                return File(bytes, contenttype, Path.GetFileName(filepath));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        private bool ContributionsExists(int id)
        {
            return _context.Contributions.Any(e => e.contributionId == id);
        }
    }
}
