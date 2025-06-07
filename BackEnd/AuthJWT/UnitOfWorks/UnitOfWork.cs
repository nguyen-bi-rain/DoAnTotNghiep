using AuthJWT.Domain.Entities;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Infrastructure.Context;
using AuthJWT.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthJWT.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserIdentity _currentUser;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context, IUserIdentity currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public ApplicationDbContext Context => _context;
        private IRepository<Hotel>? _hotelRepository;
        public IRepository<Hotel> HotelRepository => _hotelRepository ??= new Repository<Hotel>(_context, _currentUser);
        private IRepository<Rating>? _ratingRepository;
        public IRepository<Rating> RatingRepository => _ratingRepository ??= new Repository<Rating>(_context, _currentUser);
        private IRepository<Room>? _roomRepository;
        public IRepository<Room> RoomRepository => _roomRepository ??= new Repository<Room>(_context, _currentUser);
        private IRepository<RoomType>? _roomTypeRepository;
        public IRepository<RoomType> RoomTypeRepository => _roomTypeRepository ??= new Repository<RoomType>(_context, _currentUser);
        private IRepository<Convenience>? _convenienceRepository;
        public IRepository<Convenience> ConvenienceRepository => _convenienceRepository ??= new Repository<Convenience>(_context, _currentUser);
        private IRepository<Invoice>? _invoiceRepository;
        public IRepository<Invoice> InvoiceRepository => _invoiceRepository ??= new Repository<Invoice>(_context, _currentUser);
        private IRepository<Location>? _locationRepository;
        public IRepository<Location> LocationRepository => _locationRepository ??= new Repository<Location>(_context, _currentUser);
        private IRepository<RoomConvenience>? _roomConvenienceRepository;
        public IRepository<RoomConvenience> RoomConvenienceRepository => _roomConvenienceRepository ??= new Repository<RoomConvenience>(_context, _currentUser);
        private IRepository<Booking>? _bookingRepository;
        public IRepository<Booking> BookingRepository => _bookingRepository ??= new Repository<Booking>(_context, _currentUser);
        private IRepository<HotelConvenience>? _hotelConvenienceRepository;
        public IRepository<HotelConvenience> HotelConvenienceRepository => _hotelConvenienceRepository ??= new Repository<HotelConvenience>(_context, _currentUser);
        private IRepository<HotelImage>? _hotelImageRepository;
        public IRepository<HotelImage> HotelImageRepository => _hotelImageRepository ??= new Repository<HotelImage>(_context, _currentUser);
        private IRepository<RoomImage>? _roomImageRepository;
        public IRepository<RoomImage> RoomImageRepository => _roomImageRepository ??= new Repository<RoomImage>(_context, _currentUser);
        private IRepository<Policy>? _policyRepository;
        public IRepository<Policy> PolicyRepository => _policyRepository ??= new Repository<Policy>(_context, _currentUser);

        private IRepository<BookingRoom>? _bookingRoomRepository;

        public IRepository<BookingRoom> BookingRoomRepository => _bookingRoomRepository ??= new Repository<BookingRoom>(_context, _currentUser);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public IRepository<T> Repository<T>() where T : BaseEntity, IBaseEntity
        {
            return new Repository<T>(_context, _currentUser);
        }

        public void Detach<T>(T entity) where T : BaseEntity, IBaseEntity
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
    }
}