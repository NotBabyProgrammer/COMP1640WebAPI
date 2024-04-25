using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class ChatBoxes
    {
        [Key]
        public int chatId { get; set; }
        public string? facultyName { get; set; }
        //public string? userName { get; set; }
        public int userId { get; set; }
        public DateTime chatTime { get; set; }
        public string? contents { get; set; }
    }
}
