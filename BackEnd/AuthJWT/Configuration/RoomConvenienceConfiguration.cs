using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class RoomConvenienceConfiguration : IEntityTypeConfiguration<RoomConvenience>
    {
        public void Configure(EntityTypeBuilder<RoomConvenience> builder)
        {
            builder.ToTable("RoomConveniences");

            builder.HasKey(rc => rc.Id);
            builder.Property(rc => rc.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("Id");
        }
    }
}