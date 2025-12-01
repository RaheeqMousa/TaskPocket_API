using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskPocket.DAL.DTO.Requests;
using TaskPocket.DAL.Models.Enum;

namespace TaskPocketAPI.PL.Area.Tasks.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Tasks")]
    public class TasksController : ControllerBase
    {
        private readonly TaskPocket.BLL.Services.Interfaces.ITaskService _taskService;

        public TasksController(TaskPocket.BLL.Services.Interfaces.ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("")]
        [Authorize]
        public async Task<IActionResult> GetAllTasksForUserAsync([FromQuery] TaskFilter filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var tasks = await _taskService.GetAllTasksForUserAsync(userId, filter);
            return Ok(tasks);
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskByIdAsync([FromRoute] int taskId)
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            return Ok(task);
        }

        [HttpPost("")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskPocket.DAL.Models.Task task)
        {
            var taskId = await _taskService.AddTaskAsync(task);
            return Ok(taskId);
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskUpdateRequest task)
        {
            await _taskService.UpdateTaskAsync(task);
            return Ok();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute] int taskId)
        {
            var result = await _taskService.DeleteTaskAsync(taskId);
            return Ok(result);
        }

        [HttpPost("ShareTask")]
        public async Task<IActionResult> ShareTaskWithUserAsync([FromQuery] int taskId, [FromQuery] string userId)
        {
            await _taskService.ShareTaskWithUser(taskId, userId);
            return Ok();
        }



        [HttpGet("download-tasks")]
        [Authorize]
        public async Task<IActionResult> DownloadTasks()
        {
            string ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId))
                return Unauthorized();

            // Call the service to get tasks as text
            string fileContent = await _taskService.GetTasksAsTextAsync(ownerId);

            var fileBytes = Encoding.UTF8.GetBytes(fileContent);
            return File(fileBytes, "text/plain", "UserTasks.txt");
        }

    }
}