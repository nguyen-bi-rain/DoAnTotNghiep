using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(i => i.BookingId)
                .IsRequired();
            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)")
                .HasDefaultValue("Pending");

            builder.Property(i => i.IssueDate)
                .IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.Booking)
                    .WithOne()
                    .HasForeignKey<Invoice>(i => i.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}