using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class CancellationReasonConfiguration : IEntityTypeConfiguration<CancellationReason>
    {
        public void Configure(EntityTypeBuilder<CancellationReason> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()")
                .HasColumnName("Id");
            builder.Property(c => c.CancellationDate)
                .IsRequired()
                .HasColumnType("datetime");
            builder.Property(c => c.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETDATE()");

            builder.HasOne(c => c.Booking)
                .WithOne()
                .HasForeignKey<CancellationReason>(c => c.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}