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
        //public string? token { get; set; }
    }
}
