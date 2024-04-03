namespace COMP1640WebAPI.DataAccess.Models
{
    public class Messages
    {
        public int messageId {  get; set; }
        public int sendId { get; set; }
        public int receiverId { get; set; }
        public string? message { get; set; }
    }
}
