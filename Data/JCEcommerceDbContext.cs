using JC_Ecommerce.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JC_Ecommerce.Data
{
    public class JCEcommerceDbContext: DbContext
    {
        public JCEcommerceDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<Product> Products{ get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRoles (many-to-many)
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Order -> User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Order -> Status
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusId);

            // OrderItem -> Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // OrderItem -> Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            // Decimal precision configs
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            modelBuilder.Entity<Product>()
                .Property(p => p.Color)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(stringListComparer);

            modelBuilder.Entity<Product>()
                .Property(p => p.Size)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(stringListComparer);

            /*modelBuilder.Entity<Product>()
              .Property(p => p.Color)
              .HasConversion(
                  v => string.Join(',', v),
                  v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
              );*/

            /*modelBuilder.Entity<Product>()
                .Property(p => p.Size)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );*/

            //Seed data for Order Status
            //Easy, Medium, Hard

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus
                {
                    Id = Guid.Parse("ef5f3224-bdc9-4a14-8c4d-748fc5c8ca7b"),
                    Name = "Pending",
                    Color = "#facc15"
                },
                new OrderStatus
                {
                    Id = Guid.Parse("ff10af65-1d9a-44a2-bed0-e0b4e368c768"),
                    Name = "Paid",
                    Color = "#22c55e"
                },
                new OrderStatus
                {
                    Id = Guid.Parse("59af7765-7e3f-49bd-856f-a01e105bab1f"),
                    Name = "Shipped",
                    Color = "#3b82f6"
                },
                new OrderStatus
                {
                    Id = Guid.Parse("7d9b51b5-9c01-4779-a7a0-2b51385d6e6e"),
                    Name = "Delivered",
                    Color = "#10b981"
                },
                new OrderStatus
                {
                    Id = Guid.Parse("0348da95-5f3f-45cc-ab8d-8b7c3c3ada5a"),
                    Name = "Cancelled",
                    Color = "#ef4444"
                },
                new OrderStatus
                {
                    Id = Guid.Parse("75596e25-878c-455c-aba9-9f30d81d9dff"),
                    Name = "Refunded",
                    Color = "#a855f7"
                }
            );


            //Seed orderStatuses to Database.
            //modelBuilder.Entity<OrderStatus>().HasData(orderStatuses);

            var roles = new List<Role>()
            {
                new Role
                {
                    RoleId = Guid.Parse("67adb484-3f72-417c-a5f5-fc5d5c57dda8"),
                    Name = "Customer"
                },
                new Role
                {
                    RoleId = Guid.Parse("35246826-5752-4da0-8c93-45d7f9c729c3"),
                    Name = "Admin"
                }
            };

            modelBuilder.Entity<Role>().HasData(roles);

        }

    }
}
