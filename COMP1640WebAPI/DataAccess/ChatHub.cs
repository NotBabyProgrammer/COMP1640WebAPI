using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace COMP1640WebAPI.DataAccess
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderId, message);
        }
    }
}
