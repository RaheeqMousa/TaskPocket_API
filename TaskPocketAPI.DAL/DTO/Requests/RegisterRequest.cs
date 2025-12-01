using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.DTO.Requests
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
