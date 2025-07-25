using AuthJWT.Domain.Entities;
using System.Linq.Expressions;

namespace AuthJWT.Repositories
{
    public interface IRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
    {
        T? GetById(Guid id);
        Task<T> GetByIdAsync(Guid id);

        IQueryable<T> GetQueryById(Guid id);

        Task<TResult?> GetPropertyById<TResult>(Guid id, Expression<Func<T, TResult>> selector);

        IQueryable<T> GetQueryWithDeleted();
    }
}