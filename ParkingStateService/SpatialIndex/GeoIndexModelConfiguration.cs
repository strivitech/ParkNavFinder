using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ParkingStateService.SpatialIndex;

public class GeoIndexModelConfiguration : IEntityTypeConfiguration<GeoIndexModel>
{
    public void Configure(EntityTypeBuilder<GeoIndexModel> builder)
    {
        builder.HasKey(p => p.Index);
        
        builder.Property(p => p.Index)
            .HasMaxLength(16)
            .IsRequired();
    }
}