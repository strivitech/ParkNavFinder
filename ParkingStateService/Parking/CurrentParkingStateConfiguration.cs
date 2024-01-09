using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ParkingStateService.Parking;

public class CurrentParkingStateConfiguration : IEntityTypeConfiguration<CurrentParkingState>
{
    public void Configure(EntityTypeBuilder<CurrentParkingState> builder)
    {
        builder.HasKey(p => p.ParkingId);
        builder.Property(p => p.ParkingId)
            .HasMaxLength(36)
            .IsRequired();

        builder.HasIndex(p => p.Index);
        builder.Property(p => p.Index)
            .HasMaxLength(16)
            .IsRequired();
    }
}