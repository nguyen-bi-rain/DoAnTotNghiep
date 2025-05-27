using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Repositories
{
    public class GenericRepository<T, TContext> : IGenericRepository<T> where T : class where TContext : DbContext
    {
        protected readonly TContext dataContext;
        protected readonly DbSet<T> DbSet;
        public GenericRepository(TContext context)
        {
            dataContext = context;
            var typeOfDbSet = typeof(DbSet<T>);
            foreach (var prop in dataContext.GetType().GetProperties())
            {
                if (typeOfDbSet == prop.PropertyType)
                {
                    if (prop.GetValue(dataContext, null) is DbSet<T> dbSet)
                    {
                        DbSet = dbSet;
                    }
                    break;
                }
            }
            DbSet ??= dataContext.Set<T>();
        }
        #region Virtual Methods
        public virtual async Task<T> AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task AddAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                await DbSet.AddAsync(entity);
            }
        }

        public virtual void Delete(T entity, bool isHardDelete = false)
        {
            DbSet.Remove(entity);
        }

        public virtual void Delete(IEnumerable<T> entities, bool isHardDelete = false)
        {
            foreach (var entity in entities)
            {
                Delete(entity, isHardDelete);
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> where, bool isHardDelete = false)
        {
            var entities = DbSet.Where(where).AsEnumerable();
            foreach (var entity in entities)
            {
                Delete(entity, isHardDelete);
            }
        }

        public virtual IQueryable<T> GetQuery()
        {
            return DbSet.AsQueryable();
        }

        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return GetQuery().Where(where);
        }

        public virtual async void Update(T entity)
        {
            DbSet.Update(entity);
        }
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }


        #endregion
    }
}