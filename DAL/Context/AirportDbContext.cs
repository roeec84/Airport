using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DAL.Context
{
    public class AirportDbContext : DbContext
    {

        public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
        {
        }

        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Station>().HasData(
                    new Station { Id = 1, IsAvailable = true },
                    new Station { Id = 2, IsAvailable = true },
                    new Station { Id = 3, IsAvailable = true },
                    new Station { Id = 4, IsAvailable = true },
                    new Station { Id = 5, IsAvailable = true },
                    new Station { Id = 6, IsAvailable = true },
                    new Station { Id = 7, IsAvailable = true },
                    new Station { Id = 8, IsAvailable = true }
                );
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AirportDbContext>
    {
        public AirportDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "/../Server/appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<AirportDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new AirportDbContext(builder.Options);
        }
    }
}
