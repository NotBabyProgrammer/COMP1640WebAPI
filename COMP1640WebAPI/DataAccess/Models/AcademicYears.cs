using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class AcademicYears
    {
        [Key]
        public int academicYearsId { get; set; }
        public DateTime? startDays { get; set; }
        public DateTime? endDays { get; set;}
        public DateTime? finalEndDays { get; set; }
    }
}
