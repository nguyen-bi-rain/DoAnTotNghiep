using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("Id");

            builder.Property(h => h.Name)
                .IsRequired();

            builder.Property(h => h.Address)
                .IsRequired();

            builder.Property(h => h.Description);

            builder.Property(h => h.ShortDescription)
                .HasMaxLength(500);

            builder.Property(h => h.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(h => h.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(h => h.OwnerId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(h => h.IsApproved)
                .IsRequired().HasDefaultValue(false);
            builder.Property(c => c.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETDATE()");

            builder.HasOne(h => h.Location)
                .WithMany(l => l.Hotels)
                .HasForeignKey(h => h.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(h => h.Owner)
                .WithOne()
                .HasForeignKey<Hotel>(h => h.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(h => h.Bookings)
                .WithOne(b => b.Hotel)
                .HasForeignKey(b => b.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.Ratings)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.HotelImages)
                .WithOne(img => img.Hotel)
                .HasForeignKey(img => img.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.HotelConveniences)
                .WithOne(hc => hc.Hotel)
                .HasForeignKey(hc => hc.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}