using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO.Users
{
    public class UsersDTOEditEmail
    {
        [Required, EmailAddress]
        public string? email { get; set; }
    }
}
