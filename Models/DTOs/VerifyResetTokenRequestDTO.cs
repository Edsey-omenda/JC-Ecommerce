namespace JC_Ecommerce.Models.DTOs
{
    public class VerifyResetTokenRequestDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
