
using AuthJWT.Domain.Entities;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Infrastructure.Context;
using AuthJWT.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthJWT.UnitOfWorks;

/// <summary>
/// Unit of Work interface for managing database transactions and repositories.
/// </summary>
/// <remarks>
/// This interface provides methods to access repositories and manage transactions.
/// </remarks>
/// <seealso cref="IDisposable" />;

public interface IUnitOfWork : IDisposable
{
    ApplicationDbContext Context { get; }

    IRepository<Hotel> HotelRepository { get; }
    IRepository<Rating> RatingRepository { get; }
    IRepository<Room> RoomRepository { get; }
    IRepository<RoomType> RoomTypeRepository { get; }
    IRepository<Convenience> ConvenienceRepository { get; }
    IRepository<Invoice> InvoiceRepository { get; }
    IRepository<Location> LocationRepository { get; }
    IRepository<RoomConvenience> RoomConvenienceRepository { get; }
    IRepository<Booking> BookingRepository { get; }
    IRepository<HotelConvenience> HotelConvenienceRepository { get; }
    IRepository<HotelImage> HotelImageRepository { get; }
    IRepository<RoomImage> RoomImageRepository { get; }
    IRepository<Policy> PolicyRepository { get; }
    IRepository<BookingRoom> BookingRoomRepository { get; }


    IRepository<T> Repository<T>() where T : BaseEntity, IBaseEntity;

    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    void Detach<T>(T entity) where T : BaseEntity, IBaseEntity;
}
