using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using TaskPocket.BLL.Services.Interfaces;
using TaskPocket.DAL.DTO.Requests;
using TaskPocket.DAL.DTO.Responses;
using TaskPocket.DAL.Models;
using TaskPocket.DAL.Models.Enum;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.BLL.Services.Classes
{
    public class TaskService : GenericService<DAL.DTO.Requests.TaskRequest, DAL.DTO.Responses.TaskResponse, DAL.Models.Task>, ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IUserRepository _userService;
        private readonly IEmailSender _emailSender;

        public TaskService(ITaskRepository repository, IUserRepository userRepository, IEmailSender emailSender) : base(repository )
        {

            _repository = repository;
            _userService = userRepository;
            _emailSender = emailSender;
        }

        public async Task<int> AddTaskAsync(DAL.Models.Task task)
        {
            await _repository.AddTaskAsync(task);
            return task.Id;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            bool res= await _repository.DeleteTaskAsync(taskId);
            return res;
        }

        public async Task<List<TaskResponse>> GetAllTasksForUserAsync(string userId, TaskFilter filter)
        {
            var tasks = await _repository.GetAllTasksForUserAsync(userId, filter);

            var taskDtos = tasks.Select(t => new TaskResponse
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                OwnerName = !string.IsNullOrEmpty(t.Owner?.UserName) ? t.Owner.FullName : "Unknown",
                SharedWithUsers = t.SharedWithUsers
                    .Where(s => s.User != null)             // avoid null User
                    .Select(s => s.User.UserName)
                    .ToList()
            }).ToList();

            return taskDtos;
        }



        public async Task<TaskResponse?> GetTaskByIdAsync(int taskId)
        {
            var task = await _repository.GetTaskByIdAsync(taskId);
            if (task == null) return null;

            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                OwnerName = task.Owner?.UserName ?? "Unknown",
                SharedWithUsers = task.SharedWithUsers
                    .Where(s => s.User != null)
                    .Select(s => s.User.UserName)
                    .ToList()
            };
        }


        public async Task<DAL.Models.Task?> GetTaskEntityByIdAsync(int taskId)
        {
            return await _repository.GetTaskByIdAsync(taskId);
        }

    
        public async System.Threading.Tasks.Task ShareTaskWithUser(int taskId, string userId)
        {
            await _repository.ShareTaskWithUserAsync(taskId,userId);
        }

        public async Task<TaskResponse> UpdateTaskAsync(TaskUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Get the existing task from the repository
            var existingTask = await _repository.GetTaskByIdAsync(request.Id);
            if (existingTask == null)
                throw new ArgumentException("Task does not exist.");

            bool oldCompleted = existingTask.IsCompleted;

            // Update only the fields provided in the request
            if (!string.IsNullOrEmpty(request.Title))
                existingTask.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Description))
                existingTask.Description = request.Description;

            if (request.DueDate.HasValue)
                existingTask.DueDate = request.DueDate.Value;

            if (request.IsCompleted.HasValue)
                existingTask.IsCompleted = request.IsCompleted.Value;

            // Save changes
            await _repository.UpdateTaskAsync(existingTask);

            // Notify if task was just marked completed
            if (!oldCompleted && existingTask.IsCompleted)
                await NotifyTaskCompletedAsync(existingTask);

            // Return response
            return new TaskResponse
            {
                Id = existingTask.Id,
                Title = existingTask.Title,
                Description = existingTask.Description,
                DueDate = existingTask.DueDate,
                IsCompleted = existingTask.IsCompleted,
                OwnerName = existingTask.Owner?.UserName ?? "Unknown",
                SharedWithUsers = existingTask.SharedWithUsers
                    .Where(s => s.User != null)
                    .Select(s => s.User.UserName)
                    .ToList()
            };
        }



        public async System.Threading.Tasks.Task NotifyTaskCompletedAsync(DAL.Models.Task task)
        {
            Console.WriteLine($"send email");

            // Notify owner
            var owner = await _userService.GetByIdAsync(task.OwnerId);
            if (owner != null)
            {
                await _emailSender.SendEmailAsync(owner.Email, $"Task '{task.Title}' Completed!",
                    $"<p>Your task <b>{task.Title}</b> has been marked as completed.</p>");
            }

            // Notify shared users
            foreach (var sharedUserLink in task.SharedWithUsers)
            {
                var sharedUser = await _userService.GetByIdAsync(sharedUserLink.UserId);
                if (sharedUser != null)
                {
                    await _emailSender.SendEmailAsync(sharedUser.Email, $"Task '{task.Title}' Completed!",
                        $"<p>The shared task <b>{task.Title}</b> has been completed.</p>");
                }
            }
        }

        public async Task<byte[]> ExportTasksToCsvAsync(string userId, TaskFilter filter = TaskFilter.All)
        {
            var tasks = await GetAllTasksForUserAsync(userId, filter);

            var sb = new StringBuilder();

            // CSV header
            sb.AppendLine("Title,Description,DueDate,Completed,SharedWith");

            foreach (var t in tasks)
            {
                // Escape commas and quotes in text
                string title = $"\"{t.Title.Replace("\"", "\"\"")}\"";
                string description = $"\"{t.Description.Replace("\"", "\"\"")}\"";
                string dueDate = t.DueDate.ToString("yyyy-MM-dd");
                string completed = t.IsCompleted.ToString();
                string sharedWith = $"\"{string.Join(", ", t.SharedWithUsers).Replace("\"", "\"\"")}\"";

                sb.AppendLine($"{title},{description},{dueDate},{completed},{sharedWith}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }



    }
}
