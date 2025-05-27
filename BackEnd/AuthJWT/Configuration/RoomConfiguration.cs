using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("Id");

            builder.Property(x => x.RoomName)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.HotelId)
                .IsRequired();

            builder.Property(x => x.RoomTypeId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Available");

            builder.Property(x => x.ViewType)
                .IsRequired();

            builder.Property(x => x.PricePerNight)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(x => x.NumberOfBeds)
                .IsRequired();

            builder.Property(x => x.NumberOfRooms)
                .IsRequired();

            builder.Property(x => x.BedType)
                .HasMaxLength(50);

            builder.HasOne(x => x.Hotel)
                .WithMany(h => h.Rooms)
                .HasForeignKey(x => x.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.RoomType)
                .WithMany(rt => rt.Rooms)
                .HasForeignKey(x => x.RoomTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Conveniences)
                .WithOne(rc => rc.Room)
                .HasForeignKey(rc => rc.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.RoomImages)
                .WithOne(ri => ri.Room)
                .HasForeignKey(ri => ri.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}