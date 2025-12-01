using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Data;
using Microsoft.EntityFrameworkCore;
using TaskPocket.DAL.Models;
using Task = TaskPocket.DAL.Models.Task;
using TaskPocket.DAL.Repository.Interfaces;
using TaskPocket.DAL.Models.Enum;
using System.Security.Claims;

namespace TaskPocket.DAL.Repository.Classes
{
    public class TaskRepository: GenericRepository<Task>, ITaskRepository
    {
        private readonly ApplicationDBContext _context;

        public TaskRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Task>> GetAllTasksForUserAsync(string userId, TaskFilter filter)
        {
            var query = _context.Tasks
            .Include(t => t.Owner)
            .Include(t => t.SharedWithUsers)
            .Where(t =>
                t.OwnerId == userId ||
                t.SharedWithUsers.Any(u => u.UserId == userId)
            )
            .AsQueryable();

            switch (filter)
            {
                case TaskFilter.Completed:
                    query = query.Where(t => t.IsCompleted);
                    break;

                case TaskFilter.Pending:
                    query = query.Where(t => !t.IsCompleted && t.DueDate > DateTime.UtcNow);
                    break;

                case TaskFilter.Overdue:
                    query = query.Where(t => !t.IsCompleted && t.DueDate < DateTime.UtcNow);
                    break;

                case TaskFilter.DueInThreeDays:
                    var targetDate = DateTime.UtcNow.AddDays(3).Date;
                    query = query.Where(t => !t.IsCompleted && t.DueDate.Date <= targetDate);
                    break;

                case TaskFilter.Prioritized:
                    query = query.OrderBy(t => t.DueDate);
                    break;

                case TaskFilter.All:
                default:
                    break;
            }

            return await query.ToListAsync();
        }

        public async Task<Task?> GetTaskByIdAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.SharedWithUsers)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async System.Threading.Tasks.Task<int> AddTaskAsync(TaskPocket.DAL.Models.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task.Id;
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(TaskPocket.DAL.Models.Task task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async System.Threading.Tasks.Task ShareTaskWithUserAsync(int taskId, string userId)
        {
            var taskExist= await _context.Tasks.AnyAsync(t=>t.Id == taskId);
            if (!taskExist)
                throw new ArgumentException("Task does not exist.");

            var exists = await _context.TaskUsers.AnyAsync(tu => tu.TaskId == taskId && tu.User.Id == userId);
            if (!exists)
            {
                _context.TaskUsers.Add(new TaskUser { TaskId = taskId, UserId = userId });
                await _context.SaveChangesAsync();
            }else
            {
                throw new ArgumentException("Task is already shared with this user.");
            }
        }

        public async Task<List<Task>> GetTasksDueInThreeDaysAsync()
        {
            var targetDate = DateTime.UtcNow.AddDays(3).Date;

            return await _context.Tasks
                .Include(t => t.Owner)
                .Include(t => t.SharedWithUsers)
                .Where(t => !t.IsCompleted && t.DueDate.Date <= targetDate)
                .ToListAsync();
        }

    }

}

