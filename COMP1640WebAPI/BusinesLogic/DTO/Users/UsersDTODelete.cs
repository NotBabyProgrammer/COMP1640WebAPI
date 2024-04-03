using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class UsersDTODelete
    {
        [Key]
        public int userId { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public int roleId { get; set; }
    }
}
