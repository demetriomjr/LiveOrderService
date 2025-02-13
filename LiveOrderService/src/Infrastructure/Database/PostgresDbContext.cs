using Domain.Orders;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace LiveOrderService.Infrastructure.Database
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}