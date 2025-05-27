using AuthJWT.Configuration;
using AuthJWT.Domain.Entities;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<CancellationReason> CancellationReasons { get; set; }
        public DbSet<BookingRoom> BookingRooms { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Convenience> Conveniences { get; set; }
        public DbSet<RoomConvenience> RoomConveniences { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<HotelImage> HotelImages { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }
        public DbSet<HotelConvenience> HotelConveniences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new HotelConfiguration());
            builder.ApplyConfiguration(new RoomConfiguration());
            builder.ApplyConfiguration(new RoomTypeConfiguration());
            builder.ApplyConfiguration(new BookingConfiguration());
            builder.ApplyConfiguration(new BookingRoomConfiguration());
            builder.ApplyConfiguration(new HotelImageConfiguration());
            builder.ApplyConfiguration(new RoomImageConfiguration());
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new PolicyConfiguration());
            // builder.ApplyConfiguration(new CancellationReasonConfiguration());
            builder.ApplyConfiguration(new ConvenienceConfiguration());
            builder.ApplyConfiguration(new RoomConvenienceConfiguration());
            builder.ApplyConfiguration(new HotelConvenienceConfiguration());
            builder.ApplyConfiguration(new RatingConfiguration());
            builder.ApplyConfiguration(new InvoiceConfiguration());
        }

        public override int SaveChanges()
        {
            BeforeSaveChange();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChange();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void BeforeSaveChange()
        {
            var entities = this.ChangeTracker.Entries<IBaseEntity>();

            foreach (var item in entities)
            {
                switch (item.State)
                {
                    case EntityState.Added:
                        item.Entity.CreatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        item.Entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }
        }
    }
}