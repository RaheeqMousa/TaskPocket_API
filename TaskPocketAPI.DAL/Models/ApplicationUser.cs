using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;

namespace TaskPocket.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? CodeResetPassword { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }
        public string? LoginOtpCode { get; set; }
        public DateTime? LoginOtpCodeExpiry { get; set; }
    }
}
