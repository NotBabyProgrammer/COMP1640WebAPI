using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using COMP1640WebAPI.DataAccess;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.DataAccess.Data;

namespace COMP1640WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;

        public MessagesController(COMP1640WebAPIContext context)
        {
            _context = context;
        }

        // POST: api/Messages
        [HttpPost]
        public IActionResult PostMessage(int senderId, int receiverId)
        {
            var newMessage = new Messages
            {
                senderId = senderId,
                receiverId = receiverId,
                message = new List<string>()
            };

            _context.Messages.Add(newMessage);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMessage), new { id = newMessage.messageId }, newMessage);
        }

        // PUT: api/Messages/5
        [HttpPut("{id}")]
        public IActionResult PutMessage(int id, [FromBody] List<string> messageList)
        {
            var message = _context.Messages.FirstOrDefault(m => m.messageId == id);

            if (message == null)
            {
                return NotFound();
            }

            if (messageList != null)
            {
                message.message.AddRange(messageList);
                _context.SaveChanges();
            }

            return NoContent();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public IActionResult GetMessage(int id)
        {
            var message = _context.Messages.FirstOrDefault(m => m.messageId == id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message.message);
        }
    }
}
