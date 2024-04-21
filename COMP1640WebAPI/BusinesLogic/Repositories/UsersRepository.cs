using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Http.HttpResults;
using NuGet.Protocol.Plugins;
using COMP1640WebAPI.BusinesLogic.DTO.Users;

namespace COMP1640WebAPI.BusinesLogic.Repositories
{
    public class UsersRepository
    {
        private readonly COMP1640WebAPIContext _context;
        public UsersRepository(COMP1640WebAPIContext context)
        {
            _context = context;
        }
        public async Task<bool> IsRoleExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.roleId == roleId);
        }

        public async Task<Roles> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<Users> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.userName == username);
        }
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.userName == username);
        }
        public async Task<Faculties> GetFacultyByNameAsync(string facultyName)
        {
            return await _context.Faculties.FirstOrDefaultAsync(f => f.facultyName == facultyName);
        }

        public async Task<List<string>> GetUserNotifications(int userId)
        {
            // Find the user by userId
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                // Handle the case where user is not found
                return null;
            }

            // Ensure notifications list is initialized
            if (user.notifications == null)
            {
                user.notifications = new List<string>();
            }

            // Order the notifications in descending order
            var orderedNotifications = user.notifications.OrderByDescending(n => n).ToList();

            return orderedNotifications;
        }

        public async Task AddUserAsync(UsersDTO user)
        {
            var newuser = new Users
            {
                userId = user.userId,
                userName = user.userName,
                avatarPath = user.avatarPath,
                facultyName = user.facultyName,
                password = user.password,
                roleId = user.roleId,
                email = user.email,
            };
            _context.Users.Add(newuser);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserAsync(Users user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public bool IsUserExists(int id)
        {
            return _context.Users.Any(e => e.userId == id);
        }
        public string GenerateAccessToken(Users user)
        {
            return "COMP1640";
        }
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers(int page, int pageSize, string? userNameFilter)
        {
            IQueryable<Users> query = _context.Users;

            // Apply userName filtering if provided
            if (!string.IsNullOrWhiteSpace(userNameFilter))
            {
                query = query.Where(u => u.userName.Contains(userNameFilter));
            }

            // Apply paging
            var users = await query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return users;
        }
    }
}
