using JC_Ecommerce.Data;
using JC_Ecommerce.Models.Domain;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace JC_Ecommerce.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly JCEcommerceDbContext jCEcommerceDbContext;

        public UserRepository(JCEcommerceDbContext jCEcommerceDbContext )
        {
            this.jCEcommerceDbContext = jCEcommerceDbContext;
        }

        public async Task<bool> GenerateResetTokenAsync(string email)
        {
            var user = await jCEcommerceDbContext.Users.FirstOrDefaultAsync(u => u.Email == email.ToLower());

            if (user == null)
            {
                return false;
            }

            var token = Path.GetRandomFileName().Replace(".", "").Substring(0, 6).ToUpper();
            user.ResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(30);

            await jCEcommerceDbContext.SaveChangesAsync();

            Console.WriteLine($"Reset Token for Email: ${email}: ${user.ResetToken}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await jCEcommerceDbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null || user.ResetToken != token || user.ResetTokenExpires < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpires = null;

            await jCEcommerceDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await jCEcommerceDbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            return await jCEcommerceDbContext.UserRoles
           .Where(ur => ur.UserId == userId)
           .Include(ur => ur.Role)
           .Select(ur => ur.Role.Name)
           .ToListAsync();
        }

        public async Task<User> RegisterAsync(User user, string password, string[] roles)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;

            await jCEcommerceDbContext.Users.AddAsync(user);
            await jCEcommerceDbContext.SaveChangesAsync();


            // Attach roles
            var availableRoles = await jCEcommerceDbContext.Roles.ToListAsync();

            var validRoles = roles
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .ToList();

            var matchedRoles = availableRoles
               .Where(ar => validRoles.Any(vr => vr.Equals(ar.Name, StringComparison.OrdinalIgnoreCase)))
               .ToList();

            // ✅ If no valid matched role, fallback to "Customer"
            if (!matchedRoles.Any())
            {
                var customerRole = availableRoles.FirstOrDefault(r => r.Name == "Customer");
                if (customerRole != null)
                {
                    matchedRoles.Add(customerRole);
                }
            }

            foreach (var role in matchedRoles)
            {
                await jCEcommerceDbContext.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId
                });
            }

            await jCEcommerceDbContext.SaveChangesAsync();
            return user;
        }

    }
}
