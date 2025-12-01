using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskPocket.DAL.Models;
using TaskPocket.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TaskPocket.DAL.Repository.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<List<ApplicationUser>> GetAllAsync()
        {
            return _userManager.Users.ToListAsync();
        }

        public Task<ApplicationUser?> GetByIdAsync(string id)
        {
            return _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> BlockUserAsync(string email, int mins)
        {
            var user= await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            user.LockoutEnd=DateTime.UtcNow.AddMinutes(mins);
            var result=await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> IsBlockedAsync(string uderId)
        {
            var user=await _userManager.FindByIdAsync(uderId);
            if (user == null)
                return false;
            if(user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                return true;
            return false;
        }

        public async Task<bool> UnblockUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            user.LockoutEnd = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
