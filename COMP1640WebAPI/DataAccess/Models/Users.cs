using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Users
    {
        [Key]
        public int userId {  get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public int roleId { get; set; }
        public List<string>? notifications { get; set; }
        public string? facultyName { get; set; }
        public string? email {  get; set; }
    }
}
