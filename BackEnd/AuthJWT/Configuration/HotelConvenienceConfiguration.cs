using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class HotelConvenienceConfiguration : IEntityTypeConfiguration<HotelConvenience>
    {
        public void Configure(EntityTypeBuilder<HotelConvenience> builder)
        {
            builder.HasKey(hc => hc.Id);
            builder.Property(hc => hc.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("Id");

            builder.HasOne(hc => hc.Hotel)
                .WithMany(h => h.HotelConveniences)
                .HasForeignKey(hc => hc.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}