using Auth.Domain.AggregatesModel.UserAggregate;
using Infrastructure.Core;

namespace Auth.Infrastructure.Repositories
{
    public interface IUserRepo : IRepository<User, UserId>
    {

    }

    public class UserRepo : Repository<UserContext, User, UserId>, IUserRepo
    {
        public UserRepo(UserContext context) : base(context)
        {

        }
    }
}
