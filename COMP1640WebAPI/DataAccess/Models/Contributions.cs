using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Contributions
    {
        [Key]
        public int contributionId { get; set; }
        public int userId { get; set; } // Assuming you have a user ID associated with the contribution
        public string? title { get; set; }
        public string? filePath { get; set; } // For receiving Word file
        public string? imagePath { get; set; } // For receiving Image file
        public DateTime? submissionDate { get; set; }
        public DateTime? closureDate { get; set; } // closure date is 14 days after submission
        public string? status { get; set; }
        public bool? approval { get; set; }
        public int facultyId { get; set; }
    }
}
