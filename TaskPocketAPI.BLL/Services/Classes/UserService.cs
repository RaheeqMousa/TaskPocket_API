using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using TaskPocket.BLL.Services.Interfaces;
using TaskPocket.DAL.DTO.Responses;
using TaskPocket.DAL.Models;
using TaskPocket.DAL.Repository.Interfaces;

namespace TaskPocket.BLL.Services.Classes
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    RoleName = roles.FirstOrDefault(),
                });
            }
            return userDtos;
        }
        public async Task<UserDTO> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user.Adapt<UserDTO>();
        }
        public async Task<bool> BlockUserAsync(string email, int minutes)
        {
            return await _userRepository.BlockUserAsync(email, minutes);
        }
        public async Task<bool> IsBlockedAsync(string userId)
        {
            return await _userRepository.IsBlockedAsync(userId);
        }
        public async Task<bool> UnblockUserAsync(string email)
        {
            return await _userRepository.UnblockUserAsync(email);
        }
    }
}
