using LiveOrderService.Domain.Orders;
using LiveOrderService.Domain.Tokens;
using LiveOrderService.Domain.Users;
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
        public DbSet<Token> Tokens {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(o => o.Id);
            modelBuilder.Entity<Token>().HasKey(o => o.TokenKey);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
        }
    }
}