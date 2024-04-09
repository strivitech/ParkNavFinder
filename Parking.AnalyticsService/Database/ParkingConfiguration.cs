using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Parking.AnalyticsService.Database;

public class ParkingConfiguration : IEntityTypeConfiguration<Domain.Parking>
{
    public void Configure(EntityTypeBuilder<Domain.Parking> builder)
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