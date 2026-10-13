
using AmbulanceDispatch.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AmbulanceDispatch.Api.Data;

public class AmbulanceDbContext : DbContext
{
    public AmbulanceDbContext(
        DbContextOptions<AmbulanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ambulance> Ambulances => Set<Ambulance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ambulance>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.CallSign)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            
entity.Property(a => a.Equipment)
    .HasConversion(
        equipment => System.Text.Json.JsonSerializer.Serialize(
            equipment, (System.Text.Json.JsonSerializerOptions?)null),
        json => System.Text.Json.JsonSerializer.Deserialize<string[]>(
            json, (System.Text.Json.JsonSerializerOptions?)null)
            ?? Array.Empty<string>());

        });
    }
}
