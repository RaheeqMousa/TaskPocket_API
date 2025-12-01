using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskPocket.DAL.DTO.Requests
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
