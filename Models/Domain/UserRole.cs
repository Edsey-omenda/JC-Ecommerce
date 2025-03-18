namespace JC_Ecommerce.Models.Domain
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        //Navigation Properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
