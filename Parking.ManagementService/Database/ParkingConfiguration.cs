﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Parking.ManagementService.Database;

public class ParkingConfiguration : IEntityTypeConfiguration<Domain.Parking>
{
    public void Configure(EntityTypeBuilder<Domain.Parking> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(2000).IsRequired();
        builder.Property(p => p.ProviderId).IsRequired();
        builder.Property(p => p.Latitude).IsRequired();
        builder.Property(p => p.Longitude).IsRequired();
        builder.Property(p => p.TotalSpaces).IsRequired();
        
        builder.HasIndex(p => p.GeoIndex);
        
        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Country).IsRequired().HasMaxLength(100);
            address.Property(a => a.City).IsRequired().HasMaxLength(100);
            address.Property(a => a.Street).IsRequired().HasMaxLength(200);
            address.Property(a => a.StreetNumber).IsRequired().HasMaxLength(10);
        });
    }
}