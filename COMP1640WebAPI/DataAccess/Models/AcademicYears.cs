using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class AcademicYears
    {
        [Key]
        public int academicYearsId { get; set; }
        public string? academicYear {  get; set; }

        // public string 20xx-20xx (2023-2024)
        /* start Day == finalEndDays string "DateTime.startDays.Year"
         * start Day != finalEndDays string "{DateTime.startDays.Year}/{DateTime.finalEndDays.Year}"
         */
        public DateTime? startDays { get; set; }
        public DateTime? endDays { get; set;}
        public DateTime? finalEndDays { get; set; }
    }
}
