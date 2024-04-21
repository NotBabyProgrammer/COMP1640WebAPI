﻿using System;
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
using COMP1640WebAPI.BusinesLogic.DTO.Contributions;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

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
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetContributionsByUserId(int userId)
        {
            var contributions = await _context.Contributions
                                            .Where(c => c.userId == userId)
                                            .ToListAsync();

            if (contributions == null || !contributions.Any())
            {
                return NotFound();
            }

            return contributions;
        }

        // GET: api/Contributions/GetContributionsByFaculty
        [HttpGet("GetContributionsByFaculty")]
        public async Task<ActionResult<IEnumerable<Contributions>>> GetContributionsByFaculty(string facultyName)
        {
            var contributions = await _context.Contributions
                                            .Where(c => c.facultyName == facultyName)
                                            .ToListAsync();

            if (contributions == null || !contributions.Any())
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

                var studentId = await _context.Users.FindAsync(contributions.userId);
                if (contributionsDTO.approval == null)
                {
                    return BadRequest("Nullable object must have a value");
                }                

                if (contributions.approval == false && contributionsDTO.approval == true)
                {
                    // Move files to the selected folder
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
                    // Move files out of the selected folder
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
                studentId.notifications = new List<string>();
                string message = "";
                // Update properties if provided in the DTO
                contributions.approval = contributionsDTO.approval;
                if (contributions.approval == true)
                {
                    contributions.status = "Accepted";
                    message = "Your contribution has been accepted";
                    SendEmail(studentId.email, "Approval", $"Your contribution {id} has been accepted");
                }
                else if (contributions.approval == false)
                {
                    contributions.status = "Rejected";
                    message = "Your contribution has been rejected";
                    SendEmail(studentId.email, "Approval", $"Your contribution {id} has been rejected");
                }
                _context.Entry(contributions).State = EntityState.Modified;
                studentId.notifications.Add(message);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        //PUT
        [HttpPut("EditFiles/{id}")]
        public async Task<IActionResult> EditFiles (int id, List<IFormFile> files)
        {
            // int option = 0
            // option = 1
            // option = 2
            var contributions = await _context.Contributions.FindAsync(id);
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
                DeleteFile(file, contributions.contributionId);
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

            contributions.filePaths = filePaths;
            contributions.status = "Pending";

            if (contributions.approval == true)
            {
                foreach (var file in filePaths)
                {
                    MoveFile(file, $"{id}", true);
                }
            }
            _context.Entry(contributions).State = EntityState.Modified;

            var users = await _context.Users
                    .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
                    .ToListAsync();

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
                    SendEmail(u.email, "Updation", $"Student with ID {contributions.userId} just edited his/her contribution.");
                }
            }
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPut("EditImage/{id}")]
        public async Task<IActionResult> EditImage(int id, List<IFormFile> files)
        {
            var contributions = await _context.Contributions.FindAsync(id);
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
                DeleteFile(file, contributions.contributionId);
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

            contributions.imagePaths = filePaths;
            contributions.status = "Pending";


            if (contributions.approval == true)
            {
                foreach (var file in filePaths)
                {
                    MoveFile(file, $"{id}", true);
                }
            }
            _context.Entry(contributions).State = EntityState.Modified;

            var users = await _context.Users
                    .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
                    .ToListAsync();

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
                    SendEmail(u.email, "Updation", $"Student with ID {contributions.userId} just edited his/her contribution.");
                }
            }
            await _context.SaveChangesAsync();

            return NoContent();

        }

        //PUT: api/Contributions/Edit/5
        [HttpPut("EditTitle/{id}")]
        public async Task<IActionResult> EditTitle(int id, [FromForm] ContributionsDTOEdit contributionsDTO)
        {
            var contributions = await _context.Contributions.FindAsync(id);

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

            //delete existing files and images
            //foreach (var file in contributions.filePaths)
            //{
            //    DeleteFile(file, contributions.contributionId);
            //}
            //foreach (var image in contributions.imagePaths)
            //{
            //    DeleteFile(image, contributions.contributionId);
            //}

            contributions.title = contributionsDTO.title;
            contributions.status = "Pending";

            _context.Entry(contributions).State = EntityState.Modified;


            var users = await _context.Users
                    .Where(u => u.roleId == 3 && u.facultyName == contributions.facultyName)
                    .ToListAsync();

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
                    SendEmail(u.email, "Title changed", message);
                }
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //PUT: api/Contributions/Comment/5
        [HttpPut("Comment/{id}")]
        public IActionResult CommentContributions(int id, ContributionsDTOComment contributionsDTO)
        {
            var contributions = _context.Contributions.FirstOrDefault(c => c.contributionId == id);
            var user =_context.Users.FirstOrDefault(u => u.userName == contributionsDTO.userName);
            if (contributions == null)
            {
                return NotFound("Contribution not found.");
            }

            if (contributions.commentions == null)
            {
                contributions.commentions = new List<string>();
            }

            string comment = $"{contributionsDTO.userName} commented: {contributionsDTO.commentions}";
            if (user.roleId == 1)
            {
                contributions.status = "New";
            }
            else if (user.roleId == 3)
            {
                contributions.status = "Reviewed";
            }
            else
            {
                return NotFound();
            }
            //SendEmail("BodyTest");
            contributions.commentions.Add(comment);
            _context.SaveChanges();

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

                DateTime submitDate = DateTime.Now;
                
                // Check if FacultyName exists in the Faculties table
                var facultyExists = await _context.Faculties.AnyAsync(f => f.facultyName == contributionsDTO.facultyName);

                var academicYear = await _context.AcademicYears.FirstOrDefaultAsync(a => a.academicYear == contributionsDTO.academic);

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
                List<string> comments = new List<string> {};

                var contributions = new Contributions
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
                    commentions = comments,
                    academic = contributionsDTO.academic
                };

                _context.Contributions.Add(contributions);
                var users = await _context.Users
                    .Where(u => u.roleId == 3 && u.facultyName == contributionsDTO.facultyName)
                    .ToListAsync();

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
                        SendEmail(u.email, "Submittion", $"Student with ID {contributionsDTO.userId} just submitted a contribution.");
                    }

                    
                }
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetContributions", new { id = contributions.contributionId }, contributions);
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
            var contributions = await _context.Contributions.FindAsync(contributionId);
            if (contributions == null)
            {
                return NotFound();
            }

            // Delete associated files
            foreach (var filePath in contributions.filePaths)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    DeleteFile(filePath, contributionId);
                }
            }

            foreach (var imagePath in contributions.imagePaths)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    DeleteFile(imagePath, contributionId);
                }
            }

            // Remove contribution from the database
            _context.Contributions.Remove(contributions);

            // Send notification to student
            var users = await _context.Users.FindAsync(contributions.userId);
            if (users.notifications == null)
            {
                users.notifications = new List<string>();
            }

            users.notifications.Add($"Your contribution {contributions.title} has been remove");
            SendEmail(users.email, "Article deleted", $"Your contribution {contributions.title} has been remove");
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Download/5
        [HttpGet("Download/{contributionId}")]
        public async Task<IActionResult> Download(int contributionId)
        {
            try
            {
                var contribution = await _context.Contributions.FindAsync(contributionId);
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
                    AddFolderToZip(zipArchive, folderPath, "Selected");
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return File(memoryStream.ToArray(), "application/zip", "AllSelectedContents.zip");
            }
        }

        private void AddFolderToZip(ZipArchive zipArchive, string folderPath, string parentFolderName)
        {
            // Add files in the current folder to the zip archive
            foreach (var file in Directory.GetFiles(folderPath))
            {
                var fileInfo = new FileInfo(file);
                var entry = zipArchive.CreateEntry($"{parentFolderName}/{Path.GetFileName(fileInfo.FullName)}");
                using (var entryStream = entry.Open())
                {
                    using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            // Recursively add subfolders and their contents to the zip archive
            foreach (var subFolder in Directory.GetDirectories(folderPath))
            {
                var subFolderName = Path.GetFileName(subFolder);
                AddFolderToZip(zipArchive, subFolder, $"{parentFolderName}/{subFolderName}");
            }
        }

        private async Task<string> WriteFile(int count, IFormFile file, string title)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = count + "-" + title + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload", filename);
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
            var deleteFile = Path.Combine(Directory.GetCurrentDirectory(), $"API\\Upload\\", filePath);
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

        private void SendEmail(string receiveEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("conglinhoct2003@gmail.com"));
            email.To.Add(MailboxAddress.Parse(receiveEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("conglinhoct2003@gmail.com", "adcyvzgxcdyzcrwc");
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        // FUNCTIONS will be ADDED: download all selected(manager), download to view and approve(coordinator)
    }
}
