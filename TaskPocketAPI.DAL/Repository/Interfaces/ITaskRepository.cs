using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Models.Enum;

namespace TaskPocket.DAL.Repository.Interfaces
{
    public interface ITaskRepository : IGenericRepository<TaskPocket.DAL.Models.Task>
    {
        Task<List<TaskPocket.DAL.Models.Task>> GetAllTasksForUserAsync(string userId, TaskFilter filter);

        Task<TaskPocket.DAL.Models.Task?> GetTaskByIdAsync(int taskId);

        Task<int> AddTaskAsync(TaskPocket.DAL.Models.Task task);

        Task UpdateTaskAsync(TaskPocket.DAL.Models.Task task);

        Task<bool> DeleteTaskAsync(int taskId);

        Task ShareTaskWithUserAsync(int taskId, string userId);
        Task<List<TaskPocket.DAL.Models.Task>> GetTasksDueInThreeDaysAsync();

    }
}
