using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ParkingStateService.Parking;

public class ParkingStateModelConfiguration : IEntityTypeConfiguration<ParkingStateModel>
{
    public void Configure(EntityTypeBuilder<ParkingStateModel> builder)
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