using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.StaticFiles;
using COMP1640WebAPI.BusinesLogic.DTO;
using AutoMapper;
using System.IO.Compression;
using System.Net.Mime;
using COMP1640WebAPI.BusinesLogic.DTO.Contributions;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using COMP1640WebAPI.BusinesLogic.Repositories;
using COMP1640WebAPI.DataAccess.Models;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {
        private readonly ContributionsRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public ContributionsController(IWebHostEnvironment environment, ContributionsRepository repository)
        {
            _environment = environment;
            _repository = repository;
        }

        // GET: api/Contributions
        [HttpGet]
        public async Task<ActionResult> GetContributions()
        {
            var contributions = await _repository.GetAllAsync();
            return Ok(contributions);
        }

        // GET: api/Contributions/5
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetContributionsByUserId(int userId)
        {
            var contributions = await _repository.GetContributionsByUserIdAsync(userId);

            if (!contributions.Any())
            {
                return NotFound();
            }

            return Ok(contributions);
        }

        [HttpGet("Pending")]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetPendingContributions()
        {
            var contributions = await _repository.GetContributionsByStatusAsync("Pending");
            return Ok(contributions);
        }

        [HttpGet("Accepted")]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetAcceptedContributions()
        {
            var contributions = await _repository.GetContributionsByStatusAsync("Accepted");
            return Ok(contributions);
        }

        [HttpGet("Rejected")]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetRejectedContributions()
        {
            var contributions = await _repository.GetContributionsByStatusAsync("Rejected");
            return Ok(contributions);
        }

        // GET: api/Contributions/GetContributionsByFaculty
        [HttpGet("GetContributionsByFaculty")]
        public async Task<ActionResult> GetContributionsByFaculty(string facultyName)
        {
            var contributions = await _repository.GetContributionsByFacultyAsync(facultyName);

            if (!contributions.Any())
            {
                return NotFound();
            }

            return Ok(contributions);
        }

        // PUT: api/Contributions/Review/5
        [HttpPut("Review/{id}")]
        public async Task<IActionResult> ReviewContributions(int id, ContributionsDTOReview contributionsDTO)
        {
            try
            {
                var contributions = await _repository.GetContributionByIdAsync(id);
                //var contributions = await _context.Contributions.FindAsync(id);

                if (contributions == null)
                {
                    return NotFound();
                }
                var studentId = await _repository.FindUserByIdAsync(id);
                //var studentId = await _context.Users.FindAsync(contributions.userId);
                if (contributionsDTO.approval == null)
                {
                    return BadRequest("Nullable object must have a value");
                }                

                if (contributions.approval == false && contributionsDTO.approval == true)
                {
                    // Move files to the selected folder
                    foreach (var filePath in contributions.filePaths)
                    {
                        _repository.MoveFile(filePath, $"{id}", true);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        _repository.MoveFile(imagePath, $"{id}", true);
                    }
                }
                else if (contributions.approval == true && contributionsDTO.approval == false)
                {
                    // Move files out of the selected folder
                    foreach (var filePath in contributions.filePaths)
                    {
                        _repository.MoveFile(filePath, $"{id}", false);
                    }

                    // Move images to the specified directory
                    foreach (var imagePath in contributions.imagePaths)
                    {
                        _repository.MoveFile(imagePath, $"{id}", false);
                    }
                }
                studentId.notifications = new List<string>();
                string message = "";
                // Update properties if provided in the DTO
                contributions.approval = contributionsDTO.approval;
                if (contributions.approval == true)
                {
                    contributions.status = "Accepted";
                    message = "Your contribution has been accepted";
                    _repository.SendEmail(studentId.email, "Approval", $"Your contribution {id} has been accepted");
                }
                else if (contributions.approval == false)
                {
                    contributions.status = "Rejected";
                    message = "Your contribution has been rejected";
                    _repository.SendEmail(studentId.email, "Approval", $"Your contribution {id} has been rejected");
                }

                _repository.UpdateContribution(contributions);
                //_context.Entry(contributions).State = EntityState.Modified;
                studentId.notifications.Add(message);

                await _repository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //PUT: api/Contributions/EditFiles/5
        [HttpPut("EditFiles/{id}")]
        public async Task<IActionResult> EditFiles (int id, List<IFormFile> files)
        {
            var contributions = await _repository.FindContributionByIdAsync(id);
            //var contributions = await _context.Contributions.FindAsync(id);
            List<string> filePaths = new List<string>();
            int filesCount = 0;
            DateTime editTime = DateTime.Now;

            if (editTime > contributions.endDate)
            {
                return BadRequest("Cannot edit after the final closure date");
            }

            if (contributions.status == "Rejected")
            {
                return BadRequest("Cannot edit contribution (already being rejected)");
            }

            if (files == null || files.Count == 0)
            {
                return BadRequest("Files not provided.");
            }

            foreach (var file in contributions.filePaths)
            {
                _repository.DeleteFile(file, contributions.contributionId);
            }

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return BadRequest("File is empty.");
                }
                filesCount++;
                filePaths.Add(await _repository.WriteFile(filesCount, file, contributions.title));
            }

            contributions.filePaths = filePaths;
            contributions.status = "Pending";

            if (contributions.approval == true)
            {
                foreach (var file in filePaths)
                {
                    _repository.MoveFile(file, $"{id}", true);
                }
            }
            _repository.UpdateContribution(contributions);
            //_context.Entry(contributions).State = EntityState.Modified;

            var users = await _repository.GetCoordinatorsByFacultyAsync(contributions.facultyName);
            //var users = await _context.Users
            //        .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
            //        .ToListAsync();

            foreach (var u in users)
            {
                if (u != null)
                {
                    // Send notification to all Coordinators in the faculty
                    if (u.notifications == null)
                    {
                        u.notifications = new List<string>();
                    }

                    u.notifications.Add($"Student with ID {contributions.userId} just edited his/her contribution.");
                    _repository.SendEmail(u.email, "Updation", $"Student with ID {contributions.userId} just edited his/her contribution.");
                }
            }
            await _repository.SaveChangesAsync();

            return Ok(contributions.filePaths);

        }

        //PUT: api/Contributions/EditImage/5
        [HttpPut("EditImage/{id}")]
        public async Task<IActionResult> EditImage(int id, List<IFormFile> files)
        {
            var contributions = await _repository.FindContributionByIdAsync(id);
            //var contributions = await _context.Contributions.FindAsync(id);
            List<string> filePaths = new List<string>();
            int filesCount = 0;
            DateTime editTime = DateTime.Now;

            if (editTime > contributions.endDate)
            {
                return BadRequest("Cannot edit after the final closure date");
            }

            if (contributions.status == "Rejected")
            {
                return BadRequest("Cannot edit contribution (already being rejected)");
            }

            if (files == null || files.Count == 0)
            {
                return BadRequest("Files not provided.");
            }

            foreach (var file in contributions.imagePaths)
            {
                _repository.DeleteFile(file, contributions.contributionId);
            }

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    return BadRequest("File is empty.");
                }
                filesCount++;
                filePaths.Add(await _repository.WriteFile(filesCount, file, contributions.title));
            }

            contributions.imagePaths = filePaths;
            contributions.status = "Pending";


            if (contributions.approval == true)
            {
                foreach (var file in filePaths)
                {
                    _repository.MoveFile(file, $"{id}", true);
                }
            }
            _repository.UpdateContribution(contributions);
            //_context.Entry(contributions).State = EntityState.Modified;

            var users = await _repository.GetCoordinatorsByFacultyAsync(contributions.facultyName);
            //var users = await _context.Users
            //        .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
            //        .ToListAsync();

            foreach (var u in users)
            {
                if (u != null)
                {
                    // Send notification to all Coordinators in the faculty
                    if (u.notifications == null)
                    {
                        u.notifications = new List<string>();
                    }

                    u.notifications.Add($"Student with ID {contributions.userId} just edited his/her contribution.");
                    _repository.SendEmail(u.email, "Updation", $"Student with ID {contributions.userId} just edited his/her contribution.");
                }
            }
            await _repository.SaveChangesAsync();

            return Ok(contributions.imagePaths);
        }

        //PUT: api/Contributions/EditTitle/5
        [HttpPut("EditTitle/{id}")]
        public async Task<IActionResult> EditTitle(int id, [FromForm] ContributionsDTOEdit contributionsDTO)
        {
            var contributions = await _repository.FindContributionByIdAsync(id);
            //var contributions = await _context.Contributions.FindAsync(id);

            DateTime editTime = DateTime.Now;

            if (editTime > contributions.endDate)
            {
                return BadRequest("Cannot edit after the final closure date");
            }

            if (contributions.status == "Rejected")
            {
                return BadRequest("Cannot edit contribution (already being rejected)");
            }

            if (contributionsDTO.title == null)
            {
                return BadRequest("Title is null");
            }

            contributions.title = contributionsDTO.title;
            contributions.status = "Pending";

            _repository.UpdateContribution(contributions);

            var users = await _repository.GetCoordinatorsByFacultyAsync(contributions.facultyName);
            //var users = await _context.Users
            //        .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
            //        .ToListAsync();

            string message = $"Student with ID {contributions.userId} just changed his/her contribution's name {contributionsDTO.title}.";

            foreach (var u in users)
            {
                if (u != null)
                {
                    // Send notification to all Coordinators in the faculty
                    if (u.notifications == null)
                    {
                        u.notifications = new List<string>();
                    }

                    u.notifications.Add(message);
                    _repository.SendEmail(u.email, "Title changed", message);
                }
            }
            await _repository.SaveChangesAsync();
            return Ok(contributions.title);
        }
        
        // POST: api/Contributions/AddArticles
        [HttpPost("AddArticles")]
        public async Task<ActionResult<ContributionsDTO>> PostContributions([FromForm] ContributionsDTOPost contributionsDTO, List<IFormFile> files, List<IFormFile> images, CancellationToken cancellationToken)
        {
            try
            {
                if (contributionsDTO.title == null)
                {
                    return BadRequest("There are null objects");
                }

                DateTime submitDate = DateTime.Now;

                // Check if FacultyName exists in the Faculties table
                var facultyExists = await _repository.DoesFacultyExistAsync(contributionsDTO.facultyName);
                var academicYear = await _repository.GetAcademicYearByAcademicAsync(contributionsDTO.academic);
                //var academicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.academicYear == contributionsDTO.academic);

                if (submitDate < academicYear.startDays)
                {
                    return BadRequest("Too early to submit");
                }

                if (submitDate > academicYear.endDays)
                {
                    return BadRequest("Too late to submit");
                }

                if (!facultyExists)
                {
                    return NotFound("Faculty Name does not exist.");
                }
                var user = await _repository.FindUserByIdAsync(contributionsDTO.userId);
                //var user = await _context.Users.(u => u.userId == contributionsDTO.userId);

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
                foreach (var file in files)
                {
                    if (file.Length == 0)
                    {
                        return BadRequest("File is empty.");
                    }
                    filesCount++;
                    filePaths.Add(await _repository.WriteFile(filesCount, file, contributionsDTO.title));
                }

                foreach (var image in images)
                {
                    if (image.Length == 0)
                    {
                        return BadRequest("Image is empty.");
                    }
                    filesCount++;
                    imagePaths.Add(await _repository.WriteFile(filesCount, image, contributionsDTO.title));
                }
                List<string> comments = new List<string> {};

                var contribution = new ContributionsDTO
                {
                    userId = contributionsDTO.userId,
                    title = contributionsDTO.title,
                    filePaths = filePaths,
                    imagePaths = imagePaths,
                    submissionDate = DateTime.Now,
                    approvalDate = DateTime.Now.AddDays(14),
                    endDate = academicYear.finalEndDays,
                    status = "Pending",
                    approval = false,
                    facultyName = contributionsDTO.facultyName,
                    academic = contributionsDTO.academic,
                    description = "Never gonna give you up"
                };

                await _repository.AddContributionAsync(contribution);
                var users = await _repository.GetCoordinatorsByFacultyAsync(contributionsDTO.facultyName);
                //var users = await _context.Users
                //    .Where(u => u.roleId == 3 && u.facultyName == contributionsDTO.facultyName)
                //    .ToListAsync();

                foreach (var u in users)
                {
                    if (u != null)
                    {
                        // Send notification to all Coordinators in the faculty
                        if (u.notifications == null)
                        {
                            u.notifications = new List<string>();
                        }

                        u.notifications.Add($"Student with ID {contributionsDTO.userId} just submitted a contribution.");
                        if (u.email == null)
                        {
                            return BadRequest("One or more coordinators do not have an email");
                        }
                        _repository.SendEmail(u.email, "Submittion", $"Student with ID {contributionsDTO.userId} just submitted a contribution.");
                    }

                    
                }
                await _repository.SaveChangesAsync();

                return CreatedAtAction("GetContributions", new { id = contribution.contributionId }, contribution);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
                
        // DELETE: api/Contributions/ResitArticles/5
        [HttpDelete("ResitArticles/{contributionId}")]
        public async Task<IActionResult> ResitArticles(int contributionId)
        {
            var contributions = await _repository.FindContributionByIdAsync(contributionId);
            if (contributions == null)
            {
                return NotFound();
            }

            // Delete associated files
            foreach (var filePath in contributions.filePaths)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    _repository.DeleteFile(filePath, contributionId);
                }
            }

            foreach (var imagePath in contributions.imagePaths)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    _repository.DeleteFile(imagePath, contributionId);
                }
            }

            // Remove contribution from the database
            _repository.RemoveContribution(contributions);

            // Send notification to student
            var users = await _repository.FindUserByIdAsync(contributions.userId);
            if (users.notifications == null)
            {
                users.notifications = new List<string>();
            }

            users.notifications.Add($"Your contribution {contributions.title} has been remove");
            _repository.SendEmail(users.email, "Article deleted", $"Your contribution {contributions.title} has been remove");
            await _repository.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Download/5
        [HttpGet("Download/{contributionId}")]
        public async Task<IActionResult> Download(int contributionId)
        {
            try
            {
                var contribution = await _repository.FindContributionByIdAsync(contributionId);
                if (contribution == null)
                {
                    return NotFound("Contribution not found!");
                }

                var files = contribution.filePaths;
                var images = contribution.imagePaths;
                var folderPath = Path.Combine(_environment.ContentRootPath, "API", "Upload");

                // Check if files and images are not found in the regular paths
                if ((files == null || files.Count == 0 || !files.All(file => System.IO.File.Exists(Path.Combine(folderPath, file)))) &&
                    (images == null || images.Count == 0 || !images.All(image => System.IO.File.Exists(Path.Combine(folderPath, image)))))
                {
                    // Search in backup path
                    folderPath = Path.Combine(folderPath, "Selected", contributionId.ToString());
                    if ((files == null || files.Count == 0 || !files.All(file => System.IO.File.Exists(Path.Combine(folderPath, file)))) &&
                       (images == null || images.Count == 0 || !images.All(image => System.IO.File.Exists(Path.Combine(folderPath, image)))))
                    {
                        return NotFound("Contribution not found2.");
                    }
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        // Add files to zip archive
                        foreach (var file in files)
                        {
                            var filePath = Path.Combine(folderPath, file);
                            if (System.IO.File.Exists(filePath))
                            {
                                var entry = zipArchive.CreateEntry($"{Path.GetFileName(filePath)}");
                                using (var entryStream = entry.Open())
                                {
                                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                    {
                                        fileStream.CopyTo(entryStream);
                                    }
                                }
                            }
                        }

                        // Add images to zip archive
                        foreach (var image in images)
                        {
                            var imagePath = Path.Combine(folderPath, image);
                            if (System.IO.File.Exists(imagePath))
                            {
                                var entry = zipArchive.CreateEntry($"{Path.GetFileName(imagePath)}");
                                using (var entryStream = entry.Open())
                                {
                                    using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                                    {
                                        fileStream.CopyTo(entryStream);
                                    }
                                }
                            }
                        }
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return File(memoryStream.ToArray(), "application/zip", $"Contribution_{contributionId}_Files.zip");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //GET: api//DownloadAllSelected/5
        [HttpGet("DownloadAllSelected")]
        public IActionResult DownloadAllSelected()
        {
            var folderPath = Path.Combine(_environment.ContentRootPath, "API", "Upload", "Selected");
            if (!Directory.Exists(folderPath))
            {
                return NotFound("Selected folder not found!");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    _repository.AddFolderToZip(zipArchive, folderPath, "Selected");
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "application/zip", "AllSelectedContents.zip");
            }
        }

        // GET CONTRIBUTIONS WHERE STATUS = PENDING,
        // GET CONTRIBUTIONS WHERE STATUS = ACCEPTED, REJECTED
    }
}
