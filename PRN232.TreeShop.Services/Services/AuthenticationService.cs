using Microsoft.Extensions.Logging;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Repo.Repository;
using PRN232.LaptopShop.Repo.Utils;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;

namespace PRN232.LaptopShop.Services.Services
{
    public class AuthenticationService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            UnitOfWork unitOfWork,
            IPasswordService passwordService,
            ITokenService tokenService,
            ILogger<AuthenticationService> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Result<LoginResponse>> Login(LoginRequest loginRequest)
        {
            // Check if the user exists
            var user = await _unitOfWork.AccountRepo.FindAsync(u => u.Username == loginRequest.Username);
            if (user == null)
            {
                return Result<LoginResponse>.Failure(default, 401, "Authentication failed");
            }

            // Verify the password
            if (!_passwordService.VerifyPassword(user.PasswordHash, loginRequest.Password))
            {
                return Result<LoginResponse>.Failure(default, 401, "Authentication failed");
            }

            var token = _tokenService.CreateAccessToken(user);
            return Result<LoginResponse>.Success(new LoginResponse { Token = token });
        }

        public async Task<Result<RegisterResponse>> Register(RegisterRequest registerRequest)
        {
            // check username already exists
            var existingUser = await _unitOfWork.AccountRepo.FindAsync(u => u.Username == registerRequest.Username);
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure(null, 400, "Username already exists");
            }

            // check email already exists
            existingUser = await _unitOfWork.AccountRepo.FindAsync(u => u.Email == registerRequest.Email);
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure(null, 400, "Email already exists");
            }

            // create new user
            var newUser = new Account
            {
                Username = registerRequest.Username,
                PasswordHash = _passwordService.HashPassword(registerRequest.Password),
                Email = registerRequest.Email,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _unitOfWork.AccountRepo.Add(newUser);
                await _unitOfWork.SaveChangesAsync();
                var response = new RegisterResponse
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email
                };
                return Result<RegisterResponse>.Success(response, 201);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while registering the user");
                return Result<RegisterResponse>.Failure(null, 500, "An error occurred while registering the user");

            }
        }
    }
}
