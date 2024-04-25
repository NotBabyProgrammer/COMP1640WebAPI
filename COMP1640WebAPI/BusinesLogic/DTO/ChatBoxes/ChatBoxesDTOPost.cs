using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.BusinesLogic.DTO.ChatBoxes
{
    public class ChatBoxesDTOPost
    {
        public string? facultyName { get; set; }
        public int userId { get; set; }
        public string? contents { get; set; }
    }
}
