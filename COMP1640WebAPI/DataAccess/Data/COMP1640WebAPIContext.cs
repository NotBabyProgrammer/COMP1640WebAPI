using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using COMP1640WebAPI.DataAccess.Models;

namespace COMP1640WebAPI.DataAccess.Data
{
    public class COMP1640WebAPIContext : DbContext
    {
        public COMP1640WebAPIContext (DbContextOptions<COMP1640WebAPIContext> options)
            : base(options)
        {
        }

        public DbSet<COMP1640WebAPI.DataAccess.Models.Users> Users { get; set; } = default!;
        public DbSet<COMP1640WebAPI.DataAccess.Models.Roles> Roles { get; set; } = default!;
        public DbSet<COMP1640WebAPI.DataAccess.Models.Faculties> Faculties { get; set; } = default!;
        public DbSet<COMP1640WebAPI.DataAccess.Models.GuessAccounts> GuessAccounts { get; set; } = default!;
        public DbSet<COMP1640WebAPI.DataAccess.Models.Contributions> Contributions { get; set; }

        // seed data
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Roles
        //    if (!modelBuilder.Entity<Roles>().Any())
        //    {
        //        modelBuilder.Entity<Roles>().HasData(
        //            new Roles { roleId = 1, roleName = "Student" },
        //            new Roles { roleId = 2, roleName = "Manager" },
        //            new Roles { roleId = 3, roleName = "Coordinator" },
        //            new Roles { roleId = 4, roleName = "Admin" }
        //        );
        //    }

        //    // Faculties
        //    if (!modelBuilder.Entity<Faculties>().Any())
        //    {
        //        modelBuilder.Entity<Faculties>().HasData(
        //            new Faculties { facultyId = 1, facultyName = "Computer Science" },
        //            new Faculties { facultyId = 2, facultyName = "Business Administration" },
        //            new Faculties { facultyId = 3, facultyName = "Graphic Design" }
        //        );
        //    }

        //    // GuessAccounts
        //    if (!modelBuilder.Entity<GuessAccounts>().Any())
        //    {
        //        modelBuilder.Entity<GuessAccounts>().HasData(
        //            new GuessAccounts { guestId = 1, guestName = "Guess1", guestPassword = "secret123", facultyId = 1 },
        //            new GuessAccounts { guestId = 2, guestName = "Guess2", guestPassword = "secret123", facultyId = 2 },
        //            new GuessAccounts { guestId = 3, guestName = "Guess3", guestPassword = "secret123", facultyId = 3 }
        //        );
        //    }

        //    // Users
        //    if (!modelBuilder.Entity<Users>().Any())
        //    {
        //        modelBuilder.Entity<Users>().HasData(
        //            new Users { userId = 1, userName = "Student", password = "secret123", roleId = 1 },
        //            new Users { userId = 2, userName = "Manager", password = "secret123", roleId = 2 },
        //            new Users { userId = 3, userName = "Coordinator", password = "secret123", roleId = 3 },
        //            new Users { userId = 4, userName = "Administrator", password = "secret123", roleId = 4 }
        //        );
        //    }
        //}
    }
}
