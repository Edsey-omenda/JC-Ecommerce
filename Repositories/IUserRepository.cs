using JC_Ecommerce.Models.Domain;

namespace JC_Ecommerce.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> RegisterAsync(User user, string password, string[] roles);
        Task<List<string>> GetUserRolesAsync(Guid userId);
        //Task<bool> GenerateResetTokenAsync(string email);
        Task<string?> GenerateResetTokenAsync(string email);
        Task<bool> ValidateResetTokenAsync(string email, string token);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    }
}
