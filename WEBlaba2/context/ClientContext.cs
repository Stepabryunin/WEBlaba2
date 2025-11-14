using Microsoft.EntityFrameworkCore;
using WEBlaba2.models;

namespace WEBlaba2.context
{
    public class ClientContext : DbContext
    {
        public DbSet<Client> Products => Set<Client>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=AppDatabase.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Client>()
                .HasKey(p => p.ClientId);
        }
    }
}
