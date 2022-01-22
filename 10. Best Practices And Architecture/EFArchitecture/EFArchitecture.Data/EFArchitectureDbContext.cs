using EFArchitecture.Common;
using Microsoft.EntityFrameworkCore;

namespace EFArchitecture.Data
{
    public class EFArchitectureDbContext : DbContext
    {
        public EFArchitectureDbContext()
        {

        }

        public EFArchitectureDbContext(DbContextOptions options)
         : base(options)
        {

        }

        //DbSets

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConfig.CONNECTION_STRING);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Here are the composite primary keys
            //Example:
                      //modelBuilder.Entity<ProductSale>(e =>
                      //{
                      //    e.HasKey(pk => new { pk.ProductId, pk.ClientId });
                      //});
        }
    }
}
