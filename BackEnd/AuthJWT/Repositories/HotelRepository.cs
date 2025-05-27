using AuthJWT.Domain.Entities.Common;
using AuthJWT.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> RawQuery(string sql, params object[] pareObjects)
        {
            var result = await _context.Hotels.FromSqlRaw(sql, pareObjects).ToListAsync();

            return result;
        }

    }
}
