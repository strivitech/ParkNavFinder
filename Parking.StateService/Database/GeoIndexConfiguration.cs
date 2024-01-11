using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parking.StateService.Domain;

namespace Parking.StateService.Database;

public class GeoIndexConfiguration : IEntityTypeConfiguration<GeoIndex>
{
    public void Configure(EntityTypeBuilder<GeoIndex> builder)
    {
        builder.HasKey(p => p.Index);
        
        builder.Property(p => p.Index)
            .HasMaxLength(16)
            .IsRequired();
    }
}