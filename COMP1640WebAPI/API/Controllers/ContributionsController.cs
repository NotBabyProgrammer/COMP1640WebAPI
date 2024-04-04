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
using System.IO.Compression;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public ContributionsController(COMP1640WebAPIContext context, IMapper mapper, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
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
        public async Task<IActionResult> ReviewContributions(int id, ContributionsDTOReview contributionsDTO)
        {
            try
            {
                var contributions = await _context.Contributions.FindAsync(id);

                if (contributions == null)
                {
                    return NotFound();
                }

                if (contributionsDTO.approval == null)
                {
                    return BadRequest("Nullable object must have a value");
                }

                //if (contributions.approval == null)
                //{
                //    return BadRequest("Nullable object must have a value");
                //}

                if (contributions.approval == false && contributionsDTO.approval == true)
                {
                    // Move files to the specified directory
                    foreach (var filePath in contributions.filePaths)
                    {
                        MoveFile(filePath, "Files", $"{contributions.userId}", true);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        MoveFile(imagePath, "Images", $"{contributions.userId}", true);
                    }
                }
                else if (contributions.approval == true && contributionsDTO.approval == false)
                {
                    // Move files to the specified directory
                    foreach (var filePath in contributions.filePaths)
                    {
                        MoveFile(filePath, "Files", $"{contributions.userId}", false);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        MoveFile(imagePath, "Images", $"{contributions.userId}", false);
                    }
                }

                // Update properties if provided in the DTO
                contributions.approval = contributionsDTO.approval;
                contributions.comments = contributionsDTO.comments;
                _context.Entry(contributions).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
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

        // POST: api/Contributions/AddArticles
        [HttpPost("AddArticles")]
        public async Task<ActionResult<Contributions>> PostContributions([FromForm] ContributionsDTOPost contributionsDTO, List<IFormFile> files, List<IFormFile> images, CancellationToken cancellationToken)
        {
            try
            {
                // Check if FacultyName exists in the Faculties table
                var facultyExists = await _context.Faculties.AnyAsync(f => f.facultyName == contributionsDTO.facultyName);
                if (contributionsDTO.title == null)
                {
                    return BadRequest("There are null objects");
                }
                if (!facultyExists)
                {
                    return NotFound("Faculty Name does not exist.");
                }

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

        //GET: api/Contributions/Download/5
        [HttpGet("Download/{contributionId}")]
        public async Task<IActionResult> DownloadZip(int contributionId)
        {
            var contribution = await _context.Contributions.FindAsync(contributionId);
            if (contribution == null)
            {
                return NotFound("Contribution not found!");
            }

            var folderPath = Path.Combine(_environment.ContentRootPath, $"API\\Upload\\Selected\\{contribution.userId}");
            if (!Directory.Exists(folderPath))
            {
                return NotFound("Contribution not found2!");
            }
            var files = Directory.GetFiles(folderPath);
            if (files.Length == 0)
            {
                return NotFound("Contribution not found3!");
            }

            using (var memoryStream = new  MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        var entry = zipArchive.CreateEntry(fileInfo.Name);
                        using (var entryStream = entry.Open())
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/zip", $"{contributionId}.zip");
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

        private bool ContributionsExists(int id)
        {
            return _context.Contributions.Any(e => e.contributionId == id);
        }

        private void MoveFile(string filePath, string folderName, string newFolderName, bool? approval)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var sourcePath = "";
                var targetPath = "";
                if (approval == true)
                {
                    sourcePath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\{folderName}", fileName);
                    targetPath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\Selected\\{newFolderName}", fileName);
                }
                else if (approval == false)
                {
                    sourcePath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\Selected\\{newFolderName}", fileName);
                    targetPath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\{folderName}", fileName);
                }
                else
                {
                    throw new Exception("Invalid input");
                }
                // Check if the source file exists
                if (!System.IO.File.Exists(sourcePath))
                {
                    throw new FileNotFoundException("Source file not found.");
                }

                // Ensure that the target directory exists
                if (!Directory.Exists(Path.GetDirectoryName(targetPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                }

                // Move the file to the target directory
                System.IO.File.Move(sourcePath, targetPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to move file: {ex.Message}");
            }
        }
    }
}
