using System.ComponentModel.DataAnnotations;

namespace COMP1640WebAPI.DataAccess.Models
{
    public class Messages
    {
        [Key]
        public int messageId { get; set; }
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public List<string>? message { get; set; }
    }
}
