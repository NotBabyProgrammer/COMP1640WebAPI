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
        public DbSet<COMP1640WebAPI.DataAccess.Models.Contributions> Contributions { get; set; }
        public DbSet<COMP1640WebAPI.DataAccess.Models.AcademicYears> AcademicYears { get; set; }
        public DbSet<Commentions> Commentions { get; set; }
        public DbSet<ChatBoxes> ChatBoxes { get; set; }
    }
}
