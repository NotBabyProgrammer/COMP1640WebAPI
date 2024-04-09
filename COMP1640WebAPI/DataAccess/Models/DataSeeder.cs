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
                    new Users { userName = "Coordinator 1", password = "secret123", roleId = 3 },
                    new Users { userName = "Admin", password = "secret123", roleId = 4 },
                    new Users { userName = "Coordinator 2", password = "secret123", roleId = 3 },
                    new Users { userName = "Coordinator 3", password = "secret123", roleId = 3 }
                };
                _context.Users.AddRange(users);
                _context.SaveChanges();
            }
            if (!_context.Coordinators.Any())
            {
                var coordinators = new List<Coordinators>()
                {
                    new Coordinators {userId = 3, facultyId = 1},
                    new Coordinators {userId = 5, facultyId = 2},
                    new Coordinators {userId = 6, facultyId = 3}
                };
                _context.Coordinators.AddRange(coordinators);
                _context.SaveChanges();
            }
            if (!_context.AcademicYears.Any())
            {
                var aca = new List<AcademicYears>
                {
                    new AcademicYears {startYear = new DateTime(2024, 08, 10), endYear = new DateTime(2024, 05, 25)}
                };
                _context.AcademicYears.AddRange(aca);
                _context.SaveChanges();
            }
            if (!_context.ContributionsDates.Any())
            {
                var contributionsDates = new List<ContributionsDates>()
                {
                    new ContributionsDates {academicYearId = 1, startDate = new DateTime(2024, 04, 05), endDate = new DateTime(2024, 05, 06), finalEndDate = new DateTime(2024, 05, 20)}
                };
                _context.ContributionsDates.AddRange(contributionsDates);
                _context.SaveChanges();
            }
        }
    }
}
