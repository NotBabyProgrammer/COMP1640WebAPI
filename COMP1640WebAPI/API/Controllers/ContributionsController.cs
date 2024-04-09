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
using System.Net.Mime;

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
                        MoveFile(filePath, $"{id}", true);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        MoveFile(imagePath, $"{id}", true);
                    }
                }
                else if (contributions.approval == true && contributionsDTO.approval == false)
                {
                    // Move files to the specified directory
                    foreach (var filePath in contributions.filePaths)
                    {
                        MoveFile(filePath, $"{id}", false);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        MoveFile(imagePath, $"{id}", false);
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
            int filesCount = 0;
            int imagesCount = 0;

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
                filesCount++;
                filePaths.Add(await WriteFile(filesCount, file, contributions.title));
            }

            foreach (var image in images)
            {
                if (image.Length == 0)
                {
                    return BadRequest("Image is empty.");
                }
                filesCount++;
                imagePaths.Add(await WriteFile(filesCount, image, contributions.title));
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
                if (contributionsDTO.title == null)
                {
                    return BadRequest("There are null objects");
                }

                // Check if FacultyName exists in the Faculties table
                var facultyExists = await _context.Faculties.AnyAsync(f => f.facultyName == contributionsDTO.facultyName);
                //var acaYearExists = await _context.AcademicYears.AnyAsync(aca => aca.Id == contributionsDTO.academicYearId);
                var contributionsDate = await _context.ContributionsDates.FirstOrDefaultAsync();

                if (contributionsDate == null)
                {
                    // Handle the case where no ContributionsDate is found
                    return NotFound("No ContributionsDate found.");
                }

                //if (!acaYearExists)
                //{
                //    return NotFound("Academic Year does not exist.");
                //}

                if (!facultyExists)
                {
                    return NotFound("Faculty Name does not exist.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == contributionsDTO.userId);

                if (user == null)
                {
                    return NotFound("User ID does not exist.");
                }

                if (user.roleId != 1)
                {
                    return BadRequest("Only students can upload articles.");
                }

                if (files == null || files.Count == 0)
                {
                    return BadRequest("Files not provided.");
                }
                else if (images == null || images.Count == 0)
                {
                    return BadRequest("Images not provided.");
                }

                List<string> filePaths = new List<string>();
                List<string> imagePaths = new List<string>();
                int filesCount = 0;
                int imagesCount = 0;
                foreach (var file in files)
                {
                    
                    if (file.Length == 0)
                    {
                        return BadRequest("File is empty.");
                    }
                    filesCount++;
                    filePaths.Add(await WriteFile(filesCount, file, contributionsDTO.title));
                }

                foreach (var image in images)
                {
                    if (image.Length == 0)
                    {
                        return BadRequest("Image is empty.");
                    }
                    filesCount++;
                    imagePaths.Add(await WriteFile(filesCount, image, contributionsDTO.title));
                }

                var contributions = new Contributions
                {
                    userId = contributionsDTO.userId,
                    title = contributionsDTO.title,
                    filePaths = filePaths,
                    imagePaths = imagePaths,
                    submissionDate = DateTime.Now,
                    approvalDate = DateTime.Now.AddDays(14),
                    endDate = contributionsDate.finalEndDate,
                    status = "on-time",
                    approval = false,
                    facultyName = contributionsDTO.facultyName,
                    academicYearId = 1
                };

                if (contributions.submissionDate < contributionsDate.startDate || contributions.submissionDate > contributionsDate.endDate)
                {
                    return BadRequest("Cannot submit, might be too early or overdue");
                }

                if (contributionsDate.academicYearId != contributions.academicYearId)
                {
                    return BadRequest("Invalid input");
                }

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

            // Delete associated files
            foreach (var filePath in contributions.filePaths)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    DeleteFile(filePath, id);
                }
            }

            foreach (var imagePath in contributions.imagePaths)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    DeleteFile(imagePath, id);
                }
            }

            // Remove contribution from the database
            _context.Contributions.Remove(contributions);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("ConvertToMhtml/{id}")]
        public async Task<IActionResult> ConvertToMhtml(int id)
        {
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\");
            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }

            try
            {
                // Rename the files
                foreach (var filePath in contribution.filePaths)
                {
                    var newFileName = Path.ChangeExtension(Path.GetFileName(filePath), ".mhtml");
                    var newFilePath = Path.Combine(sourcePath, newFileName);
                    var oldFilePath = Path.Combine(sourcePath, filePath);
                    System.IO.File.Move(oldFilePath, newFilePath);
                }

                foreach (var imagePath in contribution.imagePaths)
                {
                    var newImagePath = Path.Combine(sourcePath, Path.ChangeExtension(Path.GetFileName(imagePath), ".mhtml"));
                    var oldImagePath = Path.Combine(sourcePath, imagePath);
                    System.IO.File.Move(oldImagePath, newImagePath);
                }

                // Generate the MHTML content
                var mhtmlContent = $"<!DOCTYPE html><html><head><title>{contribution.title}</title></head><body><h1>{contribution.title}</h1></body></html>";

                // Return the MHTML file
                var contentDisposition = new ContentDisposition
                {
                    FileName = $"{contribution.title}.mhtml",
                    Inline = true
                };
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return Content(mhtmlContent, "text/html");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

            var folderPath = Path.Combine(_environment.ContentRootPath, $"API\\Upload\\Selected\\{contribution.contributionId}");
            if (!Directory.Exists(folderPath))
            {
                return NotFound("Contribution not found2!");
            }
            var files = Directory.GetFiles(folderPath);
            if (files.Length == 0)
            {
                return NotFound("Contribution not found3!");
            }

            using (var memoryStream = new MemoryStream())
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

        private async Task<string> WriteFile(int count, IFormFile file, string title)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = count + "-" + title + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\", filename);
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

        //GET: api/Contributions/ViewFiles/5
        [HttpGet("ViewFiles/{contributionId}")]
        public async Task<IActionResult> ViewFiles(int contributionId)
        {
            try
            {
                var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "API", "Upload");
                var contribution = await _context.Contributions.FirstOrDefaultAsync(c => c.contributionId == contributionId);
                if (contribution == null)
                {
                    return NotFound("Contribution ID not found.");
                }

                var generatedFiles = new List<string>();
                var count = 0;

                foreach (var filePath in contribution.filePaths)
                {
                    var oldFilePath = Path.Combine(sourcePath, filePath);
                    if (!System.IO.File.Exists(oldFilePath))
                    {
                        return NotFound($"File '{filePath}' not found.");
                    }

                    count++;
                    var newFileName = $"{count}-{contribution.title}.mhtml";
                    generatedFiles.Add(newFileName); // Add the file name to the list

                    var newFilePath = Path.Combine(sourcePath, newFileName);

                    // Read the content of the file
                    var fileContent = await System.IO.File.ReadAllTextAsync(oldFilePath);

                    // Write the content to a new MHTML file
                    await System.IO.File.WriteAllTextAsync(newFilePath, fileContent);
                }

                foreach (var imagePath in contribution.imagePaths)
                {
                    var oldImagePath = Path.Combine(sourcePath, imagePath);
                    if (!System.IO.File.Exists(oldImagePath))
                    {
                        return NotFound($"Image file '{imagePath}' not found.");
                    }

                    count++;
                    var newFileName = $"{count}-{contribution.title}.mhtml";
                    generatedFiles.Add(newFileName); // Add the file name to the list

                    var newImagePath = Path.Combine(sourcePath, newFileName);

                    // Read the content of the image file
                    var imageContent = await System.IO.File.ReadAllBytesAsync(oldImagePath);

                    // Write the content to a new MHTML file
                    await System.IO.File.WriteAllBytesAsync(newImagePath, imageContent);
                }

                return Ok(generatedFiles); // Return the list of generated file names
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private bool ContributionsExists(int id)
        {
            return _context.Contributions.Any(e => e.contributionId == id);
        }

        private void MoveFile(string filePath, string newFolderName, bool? approval)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var sourcePath = "";
                var targetPath = "";
                if (approval == true)
                {
                    sourcePath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\", fileName);
                    targetPath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\Selected\\{newFolderName}", fileName);
                }
                else if (approval == false)
                {
                    sourcePath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\Selected\\{newFolderName}", fileName);
                    targetPath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\", fileName);
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

        private void DeleteFile(string filePath, int contributionId)
        {
            var deleteFile = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload", filePath);
            var deleteSelectedFile = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\Selected\\{contributionId}", filePath);
            if (System.IO.File.Exists(deleteFile))
            {
                System.IO.File.Delete(deleteFile);
            }
            else if (System.IO.File.Exists(deleteSelectedFile))
            {
                System.IO.File.Delete(deleteSelectedFile);
            }
            else
            {
                throw new Exception("File not found");
            }
        }
    }
}
