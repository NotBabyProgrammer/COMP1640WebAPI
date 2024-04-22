using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class ContributionsDTOPost
    {
        public int userId { get; set; } // Assuming you have a user ID associated with the contribution
        public string? title { get; set; } // students name their works
        public string? facultyName { get; set; } // students write in their faculties
        public string? academic { get; set; }
        //public string? description { get; set; }
    }
}
