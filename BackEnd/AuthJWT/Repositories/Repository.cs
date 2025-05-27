using AuthJWT.Domain.Entities;
using AuthJWT.Infrastructure.Context;

namespace AuthJWT.Repositories
{
    public class Repository<T> : RepositoryBase<T, ApplicationDbContext> where T : class,IBaseEntity
    {
        private readonly IUserIdentity _currentUser;
        public Repository(ApplicationDbContext context,IUserIdentity currentUser) : base(context)
        {
            _currentUser = currentUser;
        }

        protected override string CurrentUserId {
            get {
                if (_currentUser != null){
                    return _currentUser.UserId;
                }
                return string.Empty;
            }
        }

        protected override string CurrentUserName {
            get{
                if(_currentUser != null){
                    return _currentUser.UserName;
                }
                return "Admin";
            }
        }
    }
}