using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Http.HttpResults;

namespace COMP1640WebAPI.BusinesLogic.Repositories
{
    public class UsersRepository
    {
        private readonly COMP1640WebAPIContext _context;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly string _jwtKey;

        public UsersRepository(COMP1640WebAPIContext context)
        {
            //IHttpContextAccessor httpContextAccessor, IConfiguration configuration
            _context = context;
            //_httpContextAccessor = httpContextAccessor;
            //_jwtKey = configuration["Authentication:JwtKey"];
        }
        public async Task<bool> IsRoleExistsAsync(int roleId)
        {
            return await _context.Roles.AnyAsync(r => r.roleId == roleId);
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

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
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
    }
}
