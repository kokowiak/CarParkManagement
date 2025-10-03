using CarParkManagement.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarParkManagement.Persistence;

public sealed class CarParkManagementDbContext : DbContext
{
    public CarParkManagementDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<ParkingSpace> ParkingSpaces { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: introduce configurations if needed
        modelBuilder.Entity<ParkingSpace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            // TODO: confirm max length
            entity.Property(e => e.ParkedVehicleReg).HasMaxLength(20).IsUnicode();
            entity.Property(e => e.VehicleType).HasMaxLength(50);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        // TODO: confirm how to configure number of spaces. No requirements for now, so seed 5 spaces for testing
        modelBuilder.Entity<ParkingSpace>().HasData(
            new ParkingSpace { Id = 1 },
            new ParkingSpace { Id = 2 },
            new ParkingSpace { Id = 3 },
            new ParkingSpace { Id = 4 },
            new ParkingSpace { Id = 5 }
        );
    }
}
