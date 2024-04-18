using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO
{
    public class UsersDTOPut
    {
        // function for Admin (roleId only)
        public int roleId { get; set; }
    }
}
