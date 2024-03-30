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
                    new Roles { roleId = 1, roleName = "Student" },
                    new Roles { roleId = 2, roleName = "Manager" },
                    new Roles { roleId = 3, roleName = "Coordinator" },
                    new Roles { roleId = 4, roleName = "Admin" }
                };
                _context.Roles.AddRange(roles);
                _context.SaveChanges();
            }
            if(!_context.Faculties.Any())
            {
                var faculties = new List<Faculties>()
                {
                    new Faculties { facultyId = 1, facultyName = "Computer Science" },
                    new Faculties { facultyId = 2, facultyName = "Business Administration" },
                    new Faculties { facultyId = 3, facultyName = "Graphic Design" }
                };
                _context.Faculties.AddRange(faculties);
                _context.SaveChanges();
            }
            if (!_context.GuessAccounts.Any())
            {
                var guessAccounts = new List<GuessAccounts>()
                {
                    new GuessAccounts { guestId = 1, guestName = "Guess1", guestPassword = "secret123", facultyId = 1 },
                    new GuessAccounts { guestId = 2, guestName = "Guess2", guestPassword = "secret123", facultyId = 2 },
                    new GuessAccounts { guestId = 3, guestName = "Guess3", guestPassword = "secret123", facultyId = 3 }
                };
                _context.GuessAccounts.AddRange(guessAccounts);
                _context.SaveChanges();
            }
            if (!_context.Users.Any())
            {
                var users = new List<Users>()
                {
                    new Users { userId = 1, userName = "Student", password = "secret123", roleId = 1 },
                    new Users { userId = 2, userName = "Manager", password = "secret123", roleId = 2 },
                    new Users { userId = 3, userName = "Coordinator", password = "secret123", roleId = 3 },
                    new Users { userId = 4, userName = "Administrator", password = "secret123", roleId = 4 }
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
        }
    }
}
