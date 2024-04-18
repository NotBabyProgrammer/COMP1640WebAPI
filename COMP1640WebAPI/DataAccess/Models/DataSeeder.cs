using COMP1640WebAPI.DataAccess.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    new Roles { roleName = "Admin" },
                    new Roles {roleName = "Guess"}
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
                    new Faculties { facultyName = "Graphic Design" },
                    new Faculties { facultyName = "None"}
                };
                _context.Faculties.AddRange(faculties);
                _context.SaveChanges();
            }
            
            if (!_context.Users.Any())
            {
                var users = new List<Users>()
                {
                    new Users { userName = "Student1", password = "secret123", roleId = 1, facultyName = "Computer Science", avatarPath = "default.png", email = "thanhthanh555ak@gmail.com"},
                    new Users { userName = "Student2", password = "secret123", roleId = 1, facultyName = "Business Administration", avatarPath = "default.png", email = "thanhthanh555ak@gmail.com"},
                    new Users { userName = "Student3", password = "secret123", roleId = 1, facultyName = "Graphic Design", avatarPath = "default.png", email = "thanhthanh555ak@gmail.com"},
                    new Users { userName = "Manager", password = "secret123", roleId = 2, facultyName = "None", avatarPath = "default.png", email = "conglinhoct2003@gmail.com"},
                    new Users { userName = "Coordinator1", password = "secret123", roleId = 3, facultyName = "Computer Science", avatarPath = "default.png", email = "long.nguyen.nt2003@gmail.com"},
                    new Users { userName = "Coordinator2", password = "secret123", roleId = 3, facultyName = "Business Administration", avatarPath = "default.png", email = "long.nguyen.nt2003@gmail.com"},
                    new Users { userName = "Coordinator3", password = "secret123", roleId = 3, facultyName = "Graphic Design" , avatarPath = "default.png", email = "long.nguyen.nt2003@gmail.com"},
                    new Users { userName = "Admin", password = "secret123", roleId = 4, facultyName = "None", avatarPath = "default.png", email = "conglinhoct2003@gmail.com"},
                    new Users { userName = "Guess1", password = "secret123", roleId = 5, facultyName = "Computer Science", avatarPath = "default.png"},
                    new Users { userName = "Guess2", password = "secret123", roleId = 5, facultyName = "Business Administration" , avatarPath = "default.png"},
                    new Users { userName = "Guess3", password = "secret123", roleId = 5, facultyName = "Graphic Design", avatarPath = "default.png"}
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
            
            if (!_context.AcademicYears.Any())
            {
                var aca = new List<AcademicYears>
                {
                    new AcademicYears {startDays = new DateTime(2024, 03, 10), endDays = new DateTime(2024, 05, 10), finalEndDays = new DateTime(2024, 05, 30)}
                };
                _context.AcademicYears.AddRange(aca);
                _context.SaveChanges();
            }
        }
    }
}
