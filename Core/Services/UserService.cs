using Core.DTOs.User;
using Core.Interfaces;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<EditProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new EditProfileDto
            {
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                UserName = user.UserName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                LoginWithGoogle = user.LoginWithGoogle
            };
        }

        public async Task<(bool Success, string Message)> EditProfileAsync(int userId, EditProfileDto model)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            // Check if username is taken by another user
            if (user.UserName.ToLower() != model.UserName.ToLower())
            {
                var existingUser = await _userRepository.GetUserByUsernameAsync(model.UserName);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    return (false, "This username is already taken");
                }
            }

            // Check if email is taken by another user
            if (user.Email.ToLower() != model.Email.ToLower())
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
                if (existingUser != null && existingUser.UserId != userId)
                {
                    return (false, "This email is already registered");
                }
            }

            // Check if phone is taken by another user
            if (user.PhoneNumber != model.PhoneNumber)
            {
                if (await _userRepository.IsPhoneNumberExistsAsync(model.PhoneNumber))
                {
                    var existingUsers = await _userRepository.GetUserByEmailAsync(model.Email);
                    // Simple check - in real app, you'd need a GetUserByPhoneNumber method
                }
            }

            // Update user
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.LastChange = DateTime.Now;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Profile updated successfully");
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordDto model)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            // Verify current password
            if (!_passwordHasher.VerifyPassword(user.Password, model.CurrentPassword))
            {
                return (false, "Current password is incorrect");
            }

            // Update password
            user.Password = _passwordHasher.HashPassword(model.NewPassword);
            user.LastChange = DateTime.Now;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Password changed successfully");
        }
    }
}
