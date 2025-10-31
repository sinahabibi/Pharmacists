using Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Core.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var hashOfInput = HashPassword(providedPassword);
            return hashedPassword == hashOfInput;
        }
    }
}
