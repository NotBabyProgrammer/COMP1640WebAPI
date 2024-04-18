using COMP1640WebAPI.BusinesLogic.DTO;
using COMP1640WebAPI.BusinesLogic.Repositories;
using COMP1640WebAPI.DataAccess.Models;
using COMP1640WebAPI.DataAccess.Migrations.AuthData;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using COMP1640WebAPI.BusinesLogic.DTO.Users;

namespace COMP1640WebAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersRepository _repository;
        private readonly IMapper _mapper;
        public UsersController(UsersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            var users = await _repository.GetUsers();
            return Ok(users);
        }
        // GET: api/Users
        [HttpGet("filtering")]
        public async Task<ActionResult> GetUsers(int page = 1, int pageSize = 10, string? userNameFilter = null)
        {
            var users = await _repository.GetUsers(page, pageSize, userNameFilter);
            return Ok(users);
        }
        
        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(UsersDTOLogin user)
        {
            var existingUser = await _repository.GetUserByUsernameAsync(user.userName);

            if (existingUser == null || existingUser.password != user.password)
            {
                return NotFound("Invalid username or password.");
            }
            var accessToken = _repository.GenerateAccessToken(existingUser); // Implement this method to generate access token

            var response = new
            {
                UserId = existingUser.userId,
                RoleId = existingUser.roleId,
                UserName = existingUser.userName,
                FacultyName = existingUser.facultyName,
                Email = existingUser.email,
                AccessToken = accessToken
            };

            return Ok(response);
        }
        
        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<Users>> Register(UsersDTOPost user)
        {
            var faculty = await _repository.GetFacultyByNameAsync(user.facultyName);
            if (faculty == null)
            {
                return NotFound("Faculty name not found.");
            }

            if (await _repository.IsUsernameExistsAsync(user.userName))
            {
                return Conflict("Username existing.");
            }

            var newUser = new Users
            {
                userName = user.userName,
                password = user.password,
                facultyName = user.facultyName,
                email = user.email,
                roleId = 1, //Hardcoded roleId to 1
                avatarPath = "default.png"
            };
            await _repository.AddUserAsync(newUser);
            return CreatedAtAction(nameof(GetUsers), new { id = newUser.userId }, newUser);
        }

        [HttpGet("{userId}/notifications")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserNotifications(int userId)
        {
            var notifications = await _repository.GetUserNotifications(userId);
            var response = new
            {
                userId = userId,
                notifications = notifications
            };
            return Ok(response);
        }

        [HttpGet("ViewUserDetail/{userId}")]
        public async Task<ActionResult<Users>> ViewUser(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            var role = await _repository.GetRoleByIdAsync(user.roleId);

            var response = new
            {
                Id = user.userId,
                Username = user.userName,
                Password = user.password,
                Email = user.email,
                Role = role.roleName,
                Faculty = user.facultyName,
                Notifications = user.notifications,
            };

            return Ok(response);
        }

        // POST: api/Users/AddManagerAndCoordinator
        [HttpPost("AddManagerAndCoordinator")]
        [Authorize]
        public async Task<ActionResult<Users>> AddManagerAndCoordinator(UsersDTOAddMMMC user)
        {
            if (await _repository.IsUsernameExistsAsync(user.userName))
            {
                return Conflict("Username existing.");
            }

            if (user.roleId == 3 && user.facultyName == "None")
            {
                return BadRequest("Marketing Coordinator must be in a faculty!");
            }

            if (user.roleId == 2 && user.facultyName != "None")
            {
                return BadRequest("Manager must not be in a faculty!");
            }
            var newUser = new Users
            {
                userName = user.userName,
                password = user.password,
                roleId = user.roleId,
                facultyName = user.facultyName
            };
            await _repository.AddUserAsync(newUser);
            return CreatedAtAction(nameof(GetUsers), new { id = newUser.userId }, newUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers(int id, UsersDTOPut usersDTO)
        {
            var userToUpdate = await _repository.GetUserByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }
            if (!await _repository.IsRoleExistsAsync(usersDTO.roleId))
            {
                return BadRequest("Invalid roleId. Role does not exist.");
            }
            _mapper.Map(usersDTO, userToUpdate);
            await _repository.UpdateUserAsync(userToUpdate);
            return NoContent();
        }

        //PUT: api/Users/ChangeUsername/5
        [HttpPut("EditProfile/{id}")]
        public async Task<IActionResult> EditProfile(int id, UsersDTOEditProfile usersDTO)
        {
            var userToUpdate = await _repository.GetUserByIdAsync(id);
            if (userToUpdate == null)
            {
                return NotFound();
            }
            // Check if the username has changed and if the new username already exists
            if (await _repository.IsUsernameExistsAsync(usersDTO.userName))
            {
                return Conflict("Username existing.");
            }

            userToUpdate.userName = usersDTO.userName;
            userToUpdate.password = usersDTO.password;
            userToUpdate.email = usersDTO.email;
            await _repository.UpdateUserAsync(userToUpdate);
            return NoContent();
        }

        [HttpGet("Uploads/{userId}")]
        public async Task<IActionResult> GetAvatarImage(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            // Directory where uploaded images are stored
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "API", "ProfilePictures");

            // Full path to the requested image
            var imagePath = Path.Combine(uploadsDirectory, user.avatarPath);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found.");
            }

            // Return the image file
            var imageFileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            return File(imageFileStream, "image/jpeg"); // Adjust the content type as needed
        }

        [HttpPut("UploadImages{userId}")]
        public async Task<IActionResult> EditProfilePicture(int userId, IFormFile avatar)
        {
            var userToUpdate = await _repository.GetUserByIdAsync(userId);

            if (userToUpdate == null)
            {
                return NotFound("User not found");
            }

            if (userToUpdate.avatarPath != "default.png")
            {
                DeleteAvatar(userToUpdate.avatarPath);
            }
            userToUpdate.avatarPath = await WriteFile(avatar, userToUpdate.userName);
            await _repository.UpdateUserAsync(userToUpdate);

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            if (!_repository.IsUserExists(id))
            {
                return NotFound();
            }
            await _repository.DeleteUserAsync(id);
            return NoContent();
        }

        private async Task<string> WriteFile(IFormFile image, string userName)
        {
            string filename = "";
            try
            {
                var extension = "." + image.FileName.Split('.')[image.FileName.Split('.').Length - 1];
                filename = userName + extension;

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\ProfilePictures");

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                var exactpath = Path.Combine(Directory.GetCurrentDirectory(), $"API\\ProfilePictures", filename);
                using (var stream = new FileStream(exactpath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
            }
            return filename;
        }

        private void DeleteAvatar(string avatarPath)
        {
            var deleteFile = Path.Combine(Directory.GetCurrentDirectory(), $"API\\ProfilePictures\\", avatarPath);
            if (System.IO.File.Exists(deleteFile))
            {
                System.IO.File.Delete(deleteFile);
            }
            else
            {
                throw new Exception("File not found");
            }
        }

    }
}
