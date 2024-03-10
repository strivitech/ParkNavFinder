using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.StateService.Domain;

namespace Parking.StateService.Database;

public class ParkingStateConfiguration : IEntityTypeConfiguration<ParkingState>
{
    public void Configure(EntityTypeBuilder<ParkingState> builder)
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