using System.ComponentModel.DataAnnotations;
namespace PRN232.LaptopShop.Services.Request
{
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
