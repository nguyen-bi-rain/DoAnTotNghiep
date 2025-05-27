using AuthJWT.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthJWT.Configuration
{
    public class ConvenienceConfiguration : IEntityTypeConfiguration<Convenience>
    {
        public void Configure(EntityTypeBuilder<Convenience> builder)
        {
            builder.ToTable("Conveniences");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.CreatedAt).IsRequired().ValueGeneratedOnAdd().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.UpdatedAt).IsRequired(false).ValueGeneratedOnUpdate().HasDefaultValueSql("GETDATE()");
            builder.Property(c => c.Type).IsRequired();
            builder.HasMany(c => c.Hotels)
                .WithOne(h => h.Convenience)
                .HasForeignKey(h => h.ConvenienceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Rooms)
                .WithOne(hc => hc.Convenience)
                .HasForeignKey(hc => hc.ConvenienceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    
}