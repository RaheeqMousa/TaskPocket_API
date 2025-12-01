using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.BLL.BackgroundServices
{
    public class TaskReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendDueSoonRemindersAsync();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromHours(24), stoppingToken); // run daily
            }
        }

        private async System.Threading.Tasks.Task SendDueSoonRemindersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var taskRepo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            var userService = scope.ServiceProvider.GetRequiredService<TaskPocket.BLL.Services.Interfaces.IUserService>();
            var emailSettings = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            // Get all tasks due in 3 days
            var tasksDueSoon = await taskRepo.GetTasksDueInThreeDaysAsync();

            foreach (var task in tasksDueSoon)
            {
                // Get the owner using OwnerId
                var owner = await userService.GetByIdAsync(task.Owner.Id.ToString());
                if (owner != null && !string.IsNullOrEmpty(owner.Email))
                {
                    var subject = $"Task '{task.Title}' is due in 3 days!";
                    var body = $@"
                        <p>Hi {owner.FullName},</p>
                        <p>This is a reminder that your task <b>{task.Title}</b> is due on <b>{task.DueDate:MMMM dd, yyyy}</b>.</p>
                        <p>Please make sure to complete it before the deadline.</p>
                        <p>– TaskPocket Team</p>";

                    await emailSettings.SendEmailAsync(owner.Email, subject, body);
                }

                foreach (var sharedUser in task.SharedWithUsers)
                {
                    var subject = $"Task '{task.Title}' is due in 3 days!";
                    var body = $@"
                        <p>Hi {owner.FullName},</p>
                        <p>This is a reminder that your task <b>{task.Title}</b> is due on <b>{task.DueDate:MMMM dd, yyyy}</b>.</p>
                        <p>Please make sure to complete it before the deadline.</p>
                        <p>– TaskPocket Team</p>";

                    var user = await userService.GetByIdAsync(sharedUser.Id.ToString());
                    if (user != null) 
                        await emailSettings.SendEmailAsync(user.Email, subject,body);
                }
            }
            
        }
    }
}
