using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.DTO.Responses
{
    public class UserResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public String? Token { get; set; }
    }
}
