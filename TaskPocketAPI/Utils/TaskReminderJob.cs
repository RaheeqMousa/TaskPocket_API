using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.BLL.BackgroundServices
{
    public class TaskReminderJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskReminderJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var taskRepo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var userService = scope.ServiceProvider.GetRequiredService<TaskPocket.BLL.Services.Interfaces.IUserService>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            // Get all tasks due in 3 days
            var tasksDueSoon = await taskRepo.GetTasksDueInThreeDaysAsync();

            foreach (var task in tasksDueSoon)
            {
                // Owner
                var owner = await userService.GetByIdAsync(task.OwnerId);
                if (owner != null && !string.IsNullOrEmpty(owner.Email))
                {
                    var subject = $"Task '{task.Title}' is due in 3 days!";
                    var body = $@"
                        <p>Hi {owner.FullName},</p>
                        <p>This is a reminder that your task <b>{task.Title}</b> is due on <b>{task.DueDate:MMMM dd, yyyy}</b>.</p>
                        <p>Please make sure to complete it before the deadline.</p>
                        <p>– TaskPocket Team</p>";

                    await emailSender.SendEmailAsync(owner.Email, subject, body);
                }

                foreach (var sharedUserLink in task.SharedWithUsers)
                {
                    var sharedUser = await userService.GetByIdAsync(sharedUserLink.UserId);
                    if (sharedUser != null && !string.IsNullOrEmpty(sharedUser.Email))
                    {
                        var subject = $"Task '{task.Title}' is due in 3 days!";
                        var body = $@"
                            <p>Hi {sharedUser.FullName},</p>
                            <p>The shared task <b>{task.Title}</b> is due on <b>{task.DueDate:MMMM dd, yyyy}</b>.</p>
                            <p>– TaskPocket Team</p>";

                        await emailSender.SendEmailAsync(sharedUser.Email, subject, body);
                    }
                }
            }

            Console.WriteLine($"TaskReminderJob: Sent reminders for {tasksDueSoon.Count} tasks.");
        }
    }
}
