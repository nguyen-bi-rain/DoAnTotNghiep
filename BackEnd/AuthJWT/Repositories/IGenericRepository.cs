using System.Linq.Expressions;

namespace AuthJWT.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);

        Task AddAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity, bool isHardDelete = false);

        void Delete(IEnumerable<T> entities, bool isHardDelete = false);

        void Delete(Expression<Func<T, bool>> where, bool isHardDelete = false);

        IQueryable<T> GetQuery();

        IQueryable<T> GetQuery(Expression<Func<T, bool>> where);
        

    }
}