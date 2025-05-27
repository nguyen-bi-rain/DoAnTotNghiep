using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class HotelImageConfiguration : IEntityTypeConfiguration<HotelImage>
    {
        public void Configure(EntityTypeBuilder<HotelImage> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.ImageUrl)
                .IsRequired();

            builder.Property(i => i.ImageType)
                .HasMaxLength(50);
            builder.Property(c => c.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.Hotel)
                .WithMany(h => h.HotelImages)
                .HasForeignKey(i => i.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}