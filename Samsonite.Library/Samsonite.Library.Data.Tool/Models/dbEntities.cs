using Microsoft.EntityFrameworkCore;

namespace Samsonite.Library.Data.Tool.Models
{
    public class dbEntities : DbContext
    {
        private string _connectionStrings;
        public dbEntities(string connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public dbEntities(DbContextOptions<dbEntities> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionStrings);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DBTable>().HasNoKey();
            modelBuilder.Entity<DBTableDetail>().HasNoKey();
        }
    }
}
