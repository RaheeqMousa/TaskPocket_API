using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.Models
{
    public class TaskUser:BaseModel
    {
        public int? TaskId { get; set; }
        public Task Task { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
