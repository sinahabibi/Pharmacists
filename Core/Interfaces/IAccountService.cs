using Core.DTOs.Account;

namespace Core.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Success, string Message, int? UserId)> RegisterAsync(RegisterDto model);
        Task<(bool Success, string Message, int? UserId)> LoginAsync(LoginDto model);
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model);
        Task<(bool Success, string Message, int? UserId)> GoogleLoginAsync(string email, string name);
    }
}
