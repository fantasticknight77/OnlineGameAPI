using Microsoft.EntityFrameworkCore;
using OnlineGameAPI.Models;

namespace OnlineGameAPI.Data
{
    public class OnlineGameContext : DbContext
    {
        public OnlineGameContext(DbContextOptions<OnlineGameContext> options) : base(options)
        { 
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<SubscriptionType> Subscriptions { get; set; }
    }
}
