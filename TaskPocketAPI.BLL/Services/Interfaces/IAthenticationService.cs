using TaskPocket.DAL.DTO.Responses;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace TaskPocket.BLL.Services.Interfaces
{
    public interface IAthenticationService
    {
        Task<UserResponse> LoginAsync(DAL.DTO.Requests.LoginRequest loginRequest);

        Task<UserResponse> RegisterAsync(DAL.DTO.Requests.RegisterRequest registerRequest, HttpRequest request);

        Task<string> ConfirmEmail(string token, string userId);

        Task<bool> ForgotPassword(ForgotPasswordRequest request);

        Task<bool> ResetPassword(ResetPasswordRequest request);
        Task<bool> GenerateLoginOtpAsync(string email);
        Task<UserResponse> LoginWithOtpAsync(string email, string otp);

    }
}
