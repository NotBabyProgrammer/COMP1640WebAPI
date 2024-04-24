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
        [HttpGet]
        public async Task<IActionResult> GetChatMessagesByFacultyName(string facultyName)
        {
            var chatMessages = await _context.ChatBoxes
                .Where(c => c.facultyName == facultyName)
                .ToListAsync();

            var response = new List<Object>();

            foreach (var chatMessage in chatMessages)
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
                    FacultyName = chatMessage.facultyName,
                    UserName = chatMessage.userName,
                    Content = chatMessage.contents,
                    ChatTime = $"{chatDate} at {chatHour}"
                };

                response.Add(chatDetails);
            }

            return Ok(response);
        }

        // POST: api/ChatBox/AddMessage
        [HttpPost("AddMessage")]
        public async Task<ActionResult<ChatBoxes>> AddMessage(ChatBoxesDTOPost messageDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userName == messageDTO.userName);
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
                userName = messageDTO.userName,
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
                UserName = user?.userName,
                Content = chatMessage.contents,
                ChatTime = $"{chatDate} at {chatHour}"
            };

            return CreatedAtAction(nameof(GetChatMessagesByFacultyName), response);
        }
    }
}
