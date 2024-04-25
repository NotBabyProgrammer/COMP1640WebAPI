using AutoMapper;
using COMP1640WebAPI.BusinesLogic.DTO.ChatBoxes;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxesController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;
        private readonly IMapper _mapper;

        public ChatBoxesController(COMP1640WebAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ChatBox
        [HttpGet("{facultyName}")]
        public async Task<IActionResult> GetChatMessagesByFacultyName(string facultyName)
        {
            var chatMessages = await _context.ChatBoxes
                .Where(c => c.facultyName == facultyName)
                .ToListAsync();

            var response = new List<Object>();

            foreach (var chatMessage in chatMessages)
            {
                var user = await _context.Users.FindAsync(chatMessage.userId);
                if (user == null)
                {
                    var chatDetails = new
                    {
                        AvatarPath = "https://static.thenounproject.com/png/28755-200.png",
                        UserName = "Deleted User",
                        Content = "Message cannot be seen",
                    };
                    response.Add(chatDetails);
                }
                else
                {
                    string chatDate = $"{chatMessage.chatTime.Day}/{chatMessage.chatTime.Month}/{chatMessage.chatTime.Year}";
                    string chatHour;
                    if (chatMessage.chatTime.Minute < 10)
                    {
                        chatHour = $"{chatMessage.chatTime.Hour}:0{chatMessage.chatTime.Minute}";
                    }
                    else
                    {
                        chatHour = $"{chatMessage.chatTime.Hour}:{chatMessage.chatTime.Minute}";
                    }

                    var chatDetails = new
                    {
                        AvatarPath = $"https://localhost:7021/api/Users/Uploads/{user.userId}",
                        FacultyName = chatMessage.facultyName,
                        UserName = user.userName,
                        Content = chatMessage.contents,
                        ChatTime = $"{chatDate} at {chatHour}"
                    };
                    response.Add(chatDetails);
                }
            }

            return Ok(response);
        }

        // POST: api/ChatBox/AddMessage
        [HttpPost("AddMessage")]
        public async Task<ActionResult<ChatBoxes>> AddMessage(ChatBoxesDTOPost messageDTO)
        {
            var user = await _context.Users.FindAsync(messageDTO.userId);
            if (user == null)
            {
                return NotFound("User name not found.");
            }

            if (messageDTO.contents == null)
            {
                return BadRequest("Write something to send a message!");
            }

            string messageContent = $"{messageDTO.contents}";

            var chatMessage = new ChatBoxes
            {
                chatTime = DateTime.Now,
                userId = messageDTO.userId,
                contents = messageContent,
                facultyName = messageDTO.facultyName,
            };

            _context.ChatBoxes.Add(chatMessage);
            await _context.SaveChangesAsync();

            string chatDate = $"{chatMessage.chatTime.Day}/{chatMessage.chatTime.Month}/{chatMessage.chatTime.Year}";
            string chatHour;
            if (chatMessage.chatTime.Minute < 10)
            {
                chatHour = $"{chatMessage.chatTime.Hour}:0{chatMessage.chatTime.Minute}";
            }
            else
            {
                chatHour = $"{chatMessage.chatTime.Hour}:{chatMessage.chatTime.Minute}";
            }

            var response = new
            {
                AvatarPath = $"https://localhost:7021/api/Users/Uploads/{user.userId}",
                UserName = user.userName,
                Content = chatMessage.contents,
                ChatTime = $"{chatDate} at {chatHour}"
            };

            return CreatedAtAction(nameof(GetChatMessagesByFacultyName), new { facultyName = chatMessage.facultyName }, response);
        }
    }
}
