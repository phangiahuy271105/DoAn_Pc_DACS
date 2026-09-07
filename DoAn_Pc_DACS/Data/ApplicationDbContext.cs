using Microsoft.EntityFrameworkCore;
using DoAn_Pc_DACS.Models;

namespace DoAn_Pc_DACS.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ComponentSpec> ComponentSpecs { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Account> Accounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 99, Name = "PC GAMING", Slug = "pc-gaming" },
                new Category { Id = 100, Name = "PC WORKSTATION", Slug = "pc-workstation" }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    Username = "admin",
                    Password = "123", // Mật khẩu lúc demo bảo vệ đồ án
                    Role = "Admin"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 99,
                    CategoryId = 99,
                    Name = "PC TTG GAMING i5 12400F - RTX 5060",
                    Price = 28980000,
                    OldPrice = 29990000,
                    Discount = 3,
                    ImageUrl = "https://placehold.co/600x600?text=PC+GAMING+1"
                },
                new Product
                {
                    Id = 100,
                    CategoryId = 99,
                    Name = "PC TTG GAMING ULTRA 5 245KF",
                    Price = 38480000,
                    OldPrice = 39990000,
                    Discount = 4,
                    ImageUrl = "https://placehold.co/600x600?text=PC+GAMING+2"
                }
            );

            modelBuilder.Entity<ComponentSpec>().HasData(
                new ComponentSpec
                {
                    Id = 99,
                    ProductId = 99,
                    Socket = "Intel LGA 1700",
                    RamType = "16GB (2x8GB) DDR4 3200MHz",
                    FormFactor = "Micro-ATX",
                    Wattage = 650
                },
                new ComponentSpec
                {
                    Id = 100,
                    ProductId = 100,
                    Socket = "Intel LGA 1700 (Core Ultra)",
                    RamType = "32GB (2x16GB) DDR5 6000MHz RGB",
                    FormFactor = "ATX",
                    Wattage = 850
                }
            );
        }
    }
}