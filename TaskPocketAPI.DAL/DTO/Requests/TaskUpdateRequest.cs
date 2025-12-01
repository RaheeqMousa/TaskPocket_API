using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.DTO.Requests
{
    public class TaskUpdateRequest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
