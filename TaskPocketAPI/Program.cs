using TaskPocket.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskPocket.DAL.Models;
using TaskPocket.BLL.Services.Interfaces;
using TaskPocket.BLL.Services.Classes;
using TaskPocket.DAL.Repository.Interfaces;
using TaskPocket.DAL.Repository.Classes;
using Microsoft.AspNetCore.Identity.UI.Services;
using TaskPocket.PL.Utils;
using TaskPocket.BLL.BackgroundServices;
using Quartz;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Swashbuckle.AspNetCore;

namespace TaskPocket.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<IEmailSender, EmailSettings>();


            builder.Services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
                var jobKey = new JobKey("TaskReminderJob");
                q.AddJob<TaskReminderJob>(opts => opts.WithIdentity(jobKey));

                // Run daily at 9:00 AM UTC
                q.AddTrigger(t => t
                    .ForJob(jobKey)
                    .WithIdentity("TaskReminderJob-trigger")
                    .WithCronSchedule("0 57 1 * * ?"));
            });
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDBContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["jwtOptions:Issuer"],
                    ValidAudience = builder.Configuration["jwtOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["jwtOptions:SecretKey"]))
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
