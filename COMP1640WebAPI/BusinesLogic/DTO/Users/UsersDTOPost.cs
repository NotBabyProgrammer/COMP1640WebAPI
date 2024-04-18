using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class UsersDTOPost
    {
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? facultyName { get; set; }
        [Required, EmailAddress]
        public string? email { get; set; }
    }
}
