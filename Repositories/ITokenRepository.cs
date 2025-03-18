using JC_Ecommerce.Models.Domain;

namespace JC_Ecommerce.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);
    }
}
