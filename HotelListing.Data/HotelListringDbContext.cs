using HotelListing.API.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotelListing.API.Data
{
    public class HotelListringDbContext : IdentityDbContext<ApiUser>
    {
        public HotelListringDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new HotelConfiguration());

        }
    }

    /// <summary>
    ///  Fix Scaffolding and Migrations
    /// </summary>
    public class HotelListringDbContextFactory : IDesignTimeDbContextFactory<HotelListringDbContext>
    {
        public HotelListringDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            var optionsBuilder = new DbContextOptionsBuilder<HotelListringDbContext>();
            var conn = builder.GetConnectionString("HotelListringDbConnectionString");
            optionsBuilder.UseSqlServer(conn);
            return new HotelListringDbContext (optionsBuilder.Options);
        }
    }
}