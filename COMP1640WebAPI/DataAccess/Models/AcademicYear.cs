using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class AcademicYear
    {
        [Key]
        public int Id { get; set; }
        public DateTime? startYear { get; set; }
        public DateTime? endYear { get; set;} // the end year cannot be 11 months or more than start year
    }
}
