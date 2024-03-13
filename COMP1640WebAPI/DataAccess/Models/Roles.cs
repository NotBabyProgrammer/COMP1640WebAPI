using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Roles
    {
        [Key]
        public int roleId {  get; set; }
        public string roleName { get; set; }
    }
}
