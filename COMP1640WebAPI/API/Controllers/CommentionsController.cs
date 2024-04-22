using AutoMapper;
using COMP1640WebAPI.BusinesLogic.DTO.Commentions;
using COMP1640WebAPI.DataAccess.Data;
using COMP1640WebAPI.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentionsController : ControllerBase
    {
        private readonly COMP1640WebAPIContext _context;
        private readonly IMapper _mapper;

        public CommentionsController(COMP1640WebAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Commentions/5
        [HttpGet("{contributionId}")]
        public async Task<IActionResult> GetCommentionsByContributionId(int contributionId)
        {
            var commentions = await _context.Commentions
                .Where(c => c.contributionId == contributionId)
                .ToListAsync();

            

            /*
            * Avatar: link
            * Up: Name + commentTime.Year + / + .
            * Down: "content"
            */
            var response = new List<object>();
            foreach (var comment in commentions)
            {
                string commentHour;
                if (comment.commentTime.Minute < 10)
                {
                    commentHour = $"{comment.commentTime.Hour}:0{comment.commentTime.Minute}";
                }
                else
                {
                    commentHour = $"{comment.commentTime.Hour}:{comment.commentTime.Minute}";
                }
                string commentDate = $"{comment.commentTime.Day}/{comment.commentTime.Month}/{comment.commentTime.Year}";
                
                var user = await _context.Users.FindAsync(comment.userId);
                var commentDetails = new
                {
                    AvatarLink = $"https://localhost:7021/api/Users/Uploads/{comment.userId}",
                    UserName = user?.userName,
                    CommentTimeText = commentDate + " at " + commentHour,
                    Content = comment.contents
                };
                response.Add(commentDetails);
            }

            return Ok(response);
        }



        [HttpPost("AddComment")]
        public async Task<ActionResult<Commentions>> AddComment(CommentionsDTOPost commentDTO)
        {
            var user = await _context.Users.FindAsync(commentDTO.userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var contribution = await _context.Contributions.FindAsync(commentDTO.contributionId);
            if (contribution == null)
            {
                return NotFound("Contribution not found.");
            }

            string commentContent = $"{commentDTO.contents}";

            var comment = new Commentions
            {
                commentTime = DateTime.Now,
                userId = commentDTO.userId,
                contributionId = commentDTO.contributionId,
                contents = commentContent
            };

            _context.Commentions.Add(comment);
            await _context.SaveChangesAsync();

            string commentHour;
            if (comment.commentTime.Minute < 10)
            {
                commentHour = $"{comment.commentTime.Hour}:0{comment.commentTime.Minute}";
            }
            else
            {
                commentHour = $"{comment.commentTime.Hour}:{comment.commentTime.Minute}";
            }

            string commentDate = $"{comment.commentTime.Day}/{comment.commentTime.Month}/{comment.commentTime.Year}";
            

            var response = new
            {
                AvatarLink = $"https://localhost:7021/api/Users/Uploads/{comment.userId}",
                UserName = user?.userName,
                CommentTimeText = commentDate + " at " + commentHour,
                Content = comment.contents
            };

            return CreatedAtAction(nameof(GetCommentionsByContributionId), new { contributionId = comment.contributionId }, response);
        }

    }
}
