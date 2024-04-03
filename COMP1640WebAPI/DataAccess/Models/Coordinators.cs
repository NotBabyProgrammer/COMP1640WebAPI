using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Coordinators
    {
        [Key]
        public int coordinatorId { get; set; }
        public int userId { get; set; }
        public int facultyId { get; set; }
    }
}
