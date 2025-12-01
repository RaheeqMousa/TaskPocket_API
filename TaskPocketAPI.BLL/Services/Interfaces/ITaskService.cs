using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskPocket.BLL.Services.Classes;
using TaskPocket.DAL.DTO;
using TaskPocket.DAL.DTO.Requests;
using TaskPocket.DAL.DTO.Responses;
using TaskPocket.DAL.Models.Enum;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.BLL.Services.Interfaces
{
    public interface ITaskService: IGenericService<TaskRequest, TaskResponse, Task>
    {
        Task<List<TaskResponse>> GetAllTasksForUserAsync(string userId, TaskFilter filter);

        Task<TaskResponse?> GetTaskByIdAsync(int taskId);

        Task<int> AddTaskAsync(TaskPocket.DAL.Models.Task task);

        Task<TaskResponse> UpdateTaskAsync(TaskUpdateRequest task);

        Task<bool> DeleteTaskAsync(int taskId);
        Task ShareTaskWithUser(int taskId, string userId);
        Task<DAL.Models.Task?> GetTaskEntityByIdAsync(int taskId);
        Task NotifyTaskCompletedAsync(TaskPocket.DAL.Models.Task task);
        Task<byte[]> ExportTasksToCsvAsync(string userId, TaskFilter filter = TaskFilter.All);
        Task<string> GetTasksAsTextAsync(string ownerId);
    }
}
