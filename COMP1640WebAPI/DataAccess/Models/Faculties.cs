using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Faculties
    {
        [Key]
        public int facultyId { get; set; }
        public string? facultyName { get; set; }
    }
}
