using COMP1640WebAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

[Route("api/messages")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;

    public MessagesController(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage(int senderId, int receiverId, string message)
    {
        // Store message in database
        // Broadcast message to all clients
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", senderId, message);
        return Ok();
    }
}
