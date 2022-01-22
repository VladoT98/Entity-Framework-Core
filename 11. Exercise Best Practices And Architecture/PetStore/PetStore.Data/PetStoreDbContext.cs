using Microsoft.EntityFrameworkCore;
using PetStore.Common;
using PetStore.Models;

namespace PetStore.Data
{
    public class PetStoreDbContext : DbContext
    {
        public PetStoreDbContext()
        {

        }

        public PetStoreDbContext(DbContextOptions options)
        : base(options)
        {

        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Breed> Breeds { get; set; }
        public DbSet<CardInfo> CardsInfo { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<PetReservation> PetReservations { get; set; }
        public DbSet<PetType> PetTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSale> ProductSales { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Store> Stores { get; set; }

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
            modelBuilder.Entity<ProductSale>(e =>
            {
                e.HasKey(pk => new { pk.ProductId, pk.ClientId });
            });
        }
    }
}
