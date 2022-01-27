using Demo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo.Data
{
    public class DemoDataContext : DbContext
    {

        public DemoDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CompanyEntity> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyEntity>().ToTable("Companies");
        }
    }
}
