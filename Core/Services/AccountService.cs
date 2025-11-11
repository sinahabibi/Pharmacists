using Core.DTOs.Account;
using Core.Interfaces;
using DataLayer.Entities.User;

namespace Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AccountService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Message, int? UserId)> RegisterAsync(RegisterDto model)
        {
            // Check if username exists
            if (await _userRepository.IsUsernameExistsAsync(model.UserName))
            {
                return (false, "This username is already registered", null);
            }

            // Check if email exists
            if (await _userRepository.IsEmailExistsAsync(model.Email))
            {
                return (false, "This email is already registered", null);
            }

            // Check if phone exists
            if (await _userRepository.IsPhoneNumberExistsAsync(model.PhoneNumber))
            {
                return (false, "This phone number is already registered", null);
            }

            // Create new user
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = _passwordHasher.HashPassword(model.Password),
                ActiveCode = Guid.NewGuid().ToString().Replace("-", ""),
                SecurityCode = Guid.NewGuid().ToString().Replace("-", ""),
                ActivePhoneNumberCode = new Random().Next(10000, 99999).ToString(),
                IsEmailActive = false,
                IsPhoneNumberActive = false,
                RegisterDate = DateTime.Now,
                LastChange = DateTime.Now,
                IsDelete = false,
                IsBan = false,
                TryCount = 0
            };

            var createdUser = await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Registration completed successfully", createdUser.UserId);
        }

        public async Task<(bool Success, string Message, int? UserId)> LoginAsync(LoginDto model)
        {
            // Find user by username or email
            var user = await _userRepository.GetUserByUsernameOrEmailAsync(model.UsernameOrEmail);

            if (user == null)
            {
                return (false, "Username or password is incorrect", null);
            }

            // Check if user is banned
            if (user.IsBan)
            {
                return (false, "Your account has been banned", null);
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(user.Password, model.Password))
            {
                // Increment try count
                user.TryCount++;
                user.LastTry = DateTime.Now;

                // Ban user if try count exceeds 5
                if (user.TryCount >= 5)
                {
                    user.IsBan = true;
                }

                await _userRepository.UpdateUserAsync(user);
                await _userRepository.SaveChangesAsync();

                return (false, "Username or password is incorrect", null);
            }

            // Reset try count and update last login
            user.TryCount = 0;
            user.LastLoginDate = DateTime.Now;
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Login successful", user.UserId);
        }

        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                // Don't reveal that the user does not exist
                return (true, "If your email is registered in the system, a password recovery link will be sent to you.");
            }

            // Generate new security code
            user.SecurityCode = Guid.NewGuid().ToString().Replace("-", "");
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            // TODO: Send email with reset link
            return (true, $"Recovery link sent to email {model.Email}. Code: {user.SecurityCode}");
        }

        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null || user.SecurityCode != model.Code)
            {
                return (false, "Recovery link is invalid");
            }

            // Update password and security code
            user.Password = _passwordHasher.HashPassword(model.Password);
            user.SecurityCode = Guid.NewGuid().ToString().Replace("-", "");
            user.TryCount = 0;
            user.IsBan = false;
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Your password has been changed successfully");
        }

        public async Task<(bool Success, string Message, int? UserId)> GoogleLoginAsync(string email, string name)
        {
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Cannot retrieve email from Google", null);
            }

            // Check if user exists
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Create new user
                var nameParts = name?.Split(' ') ?? new[] { "", "" };
                user = new User
                {
                    UserName = email.Split('@')[0] + new Random().Next(1000, 9999),
                    Email = email,
                    FirstName = nameParts.Length > 0 ? nameParts[0] : "",
                    LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "",
                    Password = _passwordHasher.HashPassword(Guid.NewGuid().ToString()),
                    LoginWithGoogle = true,
                    ActiveCode = Guid.NewGuid().ToString().Replace("-", ""),
                    SecurityCode = Guid.NewGuid().ToString().Replace("-", ""),
                    ActivePhoneNumberCode = new Random().Next(10000, 99999).ToString(),
                    IsEmailActive = true, // Email is verified by Google
                    IsPhoneNumberActive = false,
                    RegisterDate = DateTime.Now,
                    LastChange = DateTime.Now,
                    IsDelete = false,
                    IsBan = false,
                    TryCount = 0
                };

                user = await _userRepository.AddUserAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            // Update last login
            user.LastLoginDate = DateTime.Now;
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Google login successful", user.UserId);
        }
    }
}
