using Microsoft.AspNetCore.Identity;

namespace PRN232.LaptopShop.Repo.Utils
{
    public interface IPasswordService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }

    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _passwordHasher = new();
        private readonly object _dummyUser = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(_dummyUser, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(_dummyUser, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}