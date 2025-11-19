using WEBlaba2.models;
using Microsoft.EntityFrameworkCore;
using WEBlaba2.Models;

namespace WEBlaba2.context
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<CartItem>()
        //        .HasOne(ci => ci.Product)
        //        .WithMany(p => p.CartItems)
        //        .HasForeignKey(ci => ci.ProductId);

        //    modelBuilder.Entity<CartItem>()
        //        .HasOne(ci => ci.Client)
        //        .WithMany()
        //        .HasForeignKey(ci => ci.ClientId);

        //    modelBuilder.Entity<Client>(entity =>
        //    {
        //        entity.HasKey(e => e.ClientId);
        //        entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
        //        entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
        //        entity.Property(e => e.MiddleName).HasMaxLength(50);
        //        entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
        //        entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
        //        entity.Property(e => e.Login).IsRequired().HasMaxLength(20);
        //        entity.Property(e => e.Password).IsRequired().HasMaxLength(100);

        //        // Уникальные индексы
        //        entity.HasIndex(e => e.Email).IsUnique();
        //        entity.HasIndex(e => e.Login).IsUnique();
        //        entity.HasIndex(e => e.Phone).IsUnique();
        //    });
        //}
    }
}