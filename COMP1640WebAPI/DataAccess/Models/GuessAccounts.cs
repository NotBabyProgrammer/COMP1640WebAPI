using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class GuessAccounts
    {
        [Key]
        public int guestId { get; set; }
        public string? guestName { get; set; }
        public string? guestPassword { get; set; }
        public int facultyId { get; set; }

    }
}
