using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Location.AnalyticsService.Domain;

namespace User.Location.AnalyticsService.Database;

public class ParkingConfiguration : IEntityTypeConfiguration<Parking>
{
    public void Configure(EntityTypeBuilder<Parking> builder)
    {
        builder.HasKey(p => p.ParkingId);
        builder.Property(p => p.ParkingId)
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(p => p.GeoIndex);
        builder.Property(p => p.GeoIndex)
            .HasMaxLength(16)
            .IsRequired();
    }
}