using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.Models
{
    public class Task : BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DueDate { get; set; }
        public string OwnerId { get; set; }

        public ApplicationUser? Owner { get; set; }
        public ICollection<TaskUser> SharedWithUsers { get; set; } = new List<TaskUser>();
    }
}
