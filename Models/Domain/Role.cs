namespace JC_Ecommerce.Models.Domain
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; } // e.g., "Admin", "Customer"

        //Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
