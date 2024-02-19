using Microsoft.EntityFrameworkCore;

namespace ORM
{
    public class ORMDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("connection_string_here");
        }
    }
}