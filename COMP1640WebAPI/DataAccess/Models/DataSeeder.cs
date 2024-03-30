using COMP1640WebAPI.DataAccess.Data;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class DataSeeder
    {
        private readonly COMP1640WebAPIContext _context;
        public DataSeeder(COMP1640WebAPIContext context)
        {
            _context = context;
        }
        public void Seed()
        {
            if(!_context.Roles.Any())
            {
                var roles = new List<Roles>()
                {
                    new Roles { roleName = "Student" },
                    new Roles { roleName = "Manager" },
                    new Roles { roleName = "Coordinator" },
                    new Roles { roleName = "Admin" }
                };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }
            if(!_context.Faculties.Any())
            {
                var faculties = new List<Faculties>()
                {
                    new Faculties { facultyName = "Computer Science" },
                    new Faculties { facultyName = "Business Administration" },
                    new Faculties { facultyName = "Graphic Design" }
                };
                _context.Faculties.AddRange(faculties);
                _context.SaveChanges();
            }
            if (!_context.GuessAccounts.Any())
            {
                var guessAccounts = new List<GuessAccounts>()
                {
                    new GuessAccounts { guestName = "Guess1", guestPassword = "secret123", facultyId = 1 },
                    new GuessAccounts { guestName = "Guess2", guestPassword = "secret123", facultyId = 2 },
                    new GuessAccounts { guestName = "Guess3", guestPassword = "secret123", facultyId = 3 }
                };
                _context.GuessAccounts.AddRange(guessAccounts);
                _context.SaveChanges();
            }
            if (!_context.Users.Any())
            {
                var users = new List<Users>()
                {
                    new Users { userName = "Student", password = "secret123", roleId = 1 },
                    new Users { userName = "Manager", password = "secret123", roleId = 2 },
                    new Users { userName = "Coordinator", password = "secret123", roleId = 3 },
                    new Users { userName = "Administrator", password = "secret123", roleId = 4 }
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
        }
    }
}
