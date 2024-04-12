using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User.SelectParkingService.Database;

public class UserParkingSelectionConfiguration : IEntityTypeConfiguration<Domain.UserParkingSelection>
{
    public void Configure(EntityTypeBuilder<Domain.UserParkingSelection> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.UserId)
            .IsRequired();
    }
}