namespace JC_Ecommerce.Models.Domain
{
    public class User
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ResetToken { get; set; }

        public DateTime? ResetTokenExpires { get; set; }

        //Navigation properties
        public ICollection<UserRole> UserRoles { get; set; }  //Many-to-many join setup
        public ICollection<Order> Orders { get; set; } //Navigation from User → Order (one-to-many)
    }
}
