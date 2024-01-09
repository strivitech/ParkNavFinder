using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ParkingStateService.Models;

namespace ParkingStateService.Database;

public class ActiveIndexConfiguration : IEntityTypeConfiguration<ActiveIndex>
{
    public void Configure(EntityTypeBuilder<ActiveIndex> builder)
    {
        builder.HasKey(p => p.Index);
        
        builder.Property(p => p.Index)
            .HasMaxLength(16)
            .IsRequired();
    }
}

public class ActiveParkingStateConfiguration : IEntityTypeConfiguration<ActiveParkingState>
{
    public void Configure(EntityTypeBuilder<ActiveParkingState> builder)
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