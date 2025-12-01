using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskPocket.DAL.Models.Enum;

namespace TaskPocketAPI.PL.Area.Users.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Users")]
    public class UsersController : ControllerBase
    {
        private readonly TaskPocket.BLL.Services.Interfaces.IUserService _userService;

        public UsersController(TaskPocket.BLL.Services.Interfaces.IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync([FromRoute] string id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpPost("BlockUser")]
        public async Task<IActionResult> BlockUserAsync([FromQuery] string email, [FromQuery] int minutes)
        {
            var result = await _userService.BlockUserAsync(email, minutes);
            return Ok(result);
        }

        [HttpPost("UnblockUser")]
        public async Task<IActionResult> UnblockUserAsync([FromQuery] string email)
        {
            var result = await _userService.UnblockUserAsync(email);
            return Ok(result);
        }

        [HttpGet("IsBlocked/{userId}")]
        public async Task<IActionResult> IsBlockedAsync([FromRoute] string userId)
        {
            var result = await _userService.IsBlockedAsync(userId);
            return Ok(result);
        }
    }
}