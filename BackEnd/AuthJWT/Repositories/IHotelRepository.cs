using AuthJWT.Domain.Entities.Common;

namespace AuthJWT.Repositories
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> RawQuery(string sql, params object[] pareObjects);
    }
}
