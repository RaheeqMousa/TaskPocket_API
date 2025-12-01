using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Models;

namespace TaskPocket.DAL.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser> GetByIdAsync(string id);

        Task<bool> BlockUserAsync(string email, int minutes);

        Task<bool> UnblockUserAsync(string email);
        Task<bool> IsBlockedAsync(string userId);
    }
}
