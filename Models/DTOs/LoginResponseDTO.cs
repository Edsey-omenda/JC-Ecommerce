
namespace JC_Ecommerce.Models.DTOs
{
    public class LoginResponseDTO
    {
        public string JwtToken { get; set; }

        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string[] Roles { get; set; }
    }
}
