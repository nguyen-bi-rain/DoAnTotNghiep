using System.Linq.Expressions;
using AuthJWT.Domain.Entities;
using AuthJWT.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Repositories
{
    public abstract class RepositoryBase<T, TContext> : GenericRepository<T, TContext>, IRepository<T>
        where T : class, IBaseEntity
        where TContext : ApplicationDbContext
    {
        protected RepositoryBase(TContext context) : base(context)
        {
        }
        protected abstract string CurrentUserId { get; }

        protected abstract string CurrentUserName { get; }

        #region Overwrite Methods

        public override async Task<T> AddAsync(T entity)
        {
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            await DbSet.AddAsync(entity);
            return entity;
        }

        public override void Update(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            UpdateEntityObject(entity);
        }

        public override void Delete(T entity, bool isHardDelete = false)
        {

            DbSet.Remove(entity);
        }

        public override void Delete(Expression<Func<T, bool>> where, bool isHardDelete = false)
        {
            var entities = GetQuery(where).AsEnumerable();
            foreach (var entity in entities)
            {
                Delete(entity, isHardDelete);
            }
        }

        public IQueryable<T> GetQueryById(Guid id)
        {
            return GetQuery(m => m.Id == id);
        }

        public Task<TResult?> GetPropertyById<TResult>(Guid id,
            Expression<Func<T, TResult>> selector)
        {
            return GetQueryById(id).Select(selector).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetQueryWithDeleted()
        {
            return GetQuery();
        }

        public T? Refresh(T entity)
        {
            dataContext.Entry(entity).State = EntityState.Detached;
            return GetById(entity.Id);
        }

        #endregion

        #region Private Methods

        private void UpdateEntityObject(T entity)
        {
            DbSet.Attach(entity);
            entity.UpdatedAt = DateTime.UtcNow;
            dataContext.Entry(entity).State = EntityState.Modified;
            dataContext.Entry(entity).GetDatabaseValues()?.ToObject();
        }

        public T? GetById(Guid id)
        {
            var entity = GetQueryById(id).AsNoTracking().FirstOrDefault();
            if (entity == null) return null;
            dataContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        #endregion
    }
}