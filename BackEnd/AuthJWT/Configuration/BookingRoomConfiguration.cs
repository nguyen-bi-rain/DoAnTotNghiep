using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class BookingRoomConfiguration : IEntityTypeConfiguration<BookingRoom>
    {
        public void Configure(EntityTypeBuilder<BookingRoom> builder)
        {
            builder.HasKey(br => new { br.BookingId, br.RoomId });
            builder.HasOne(br => br.Booking)
                .WithMany(b => b.BookingRooms)
                .HasForeignKey(br => br.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(br => br.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(br => br.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}