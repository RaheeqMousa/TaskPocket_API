using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskPocket.DAL.Models;

namespace TaskPocket.DAL.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<TaskPocket.DAL.Models.Task> Tasks { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // When a Task is deleted, delete shared records automatically
            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.Task)
                .WithMany(t => t.SharedWithUsers)
                .HasForeignKey(tu => tu.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // When a User is deleted, do not delete tasks
            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.User)
                .WithMany()
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
