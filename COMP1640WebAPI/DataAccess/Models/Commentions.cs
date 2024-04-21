using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Commentions
    {
        [Key]
        public int commentId { get; set; }
        public int contributionId { get; set; }
        public int userId { get; set; }
        public DateTime commentTime { get; set; }
        public string? contents { get; set; }
    }
}
