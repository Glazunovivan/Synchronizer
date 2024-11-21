using Microsoft.EntityFrameworkCore;
using Synchronizer.Models;

namespace Synchronizer.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<StockCreds> StockCreds { get; set; }
        public DbSet<UserStock> UserStocks { get; set; }
        public DbSet<Error_log> ErrorsLog { get; set; }

        private readonly string _connectionString;

        public ApplicationContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
