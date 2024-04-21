using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using COMP1640WebAPI.BusinesLogic.DTO.Contributions;
using COMP1640WebAPI.BusinesLogic.DTO;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace COMP1640WebAPI.BusinesLogic.Repositories
{
    public class ContributionsRepository
    {
        private readonly COMP1640WebAPIContext _context;

        public ContributionsRepository(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contributions>> GetAllAsync()
        {
            return await _context.Contributions.ToListAsync();
        }

        public async Task<IEnumerable<Contributions>> GetContributionsByUserIdAsync(int userId)
        {
            return await _context.Contributions
                .Where(c => c.userId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contributions>> GetContributionsByFacultyAsync(string facultyName)
        {
            return await _context.Contributions
                                .Where(c => c.facultyName == facultyName)
                                .ToListAsync();
        }

        public async Task<Contributions> FindContributionByIdAsync(int contributionId)
        {
            return await _context.Contributions.FindAsync(contributionId);
        }

        public async Task<Contributions> GetContributionByIdAsync(int id)
        {
            return await _context.Contributions.FirstOrDefaultAsync(c => c.contributionId == id);
        }

        public async Task<Users> FindUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.userName == username);
        }

        public async Task<bool> DoesFacultyExistAsync(string facultyName)
        {
            return await _context.Faculties.AnyAsync(f => f.facultyName == facultyName);
        }

        public async Task<AcademicYears> GetAcademicYearByAcademicAsync(string academicYear)
        {
            return await _context.AcademicYears.FirstOrDefaultAsync(a => a.academicYear == academicYear);
        }

        public async Task<List<Users>> GetCoordinatorsByFacultyAsync(string facultyName)
        {
            return await _context.Users
                .Where(u => u.roleId == 3 && u.facultyName == facultyName)
                .ToListAsync();
        }

        public async Task AddContributionAsync(Contributions contribution)
        {
            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void UpdateContribution(Contributions contribution)
        {
            _context.Entry(contribution).State = EntityState.Modified;
        }

        public void RemoveContribution(Contributions contribution)
        {
            _context.Contributions.Remove(contribution);
        }

        public void MoveFile(string filePath, string newFolderName, bool? approval)
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

        public void SendEmail(string receiveEmail, string subject, string body)
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

        public void DeleteFile(string filePath, int contributionId)
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

        public async Task<string> WriteFile(int count, IFormFile file, string title)
        {
            string filename = "";
            try
            {
                var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                filename = count + "- User" + title + extension;

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

        public void AddFolderToZip(ZipArchive zipArchive, string folderPath, string parentFolderName)
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
    }
}
