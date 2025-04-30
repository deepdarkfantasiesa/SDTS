using Infrastructure.Core;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure.Repositories
{
    public interface IUserRepo : IRepository<Users, UserId>
    {

    }

    public class UserRepo : Repository<UserContext, Users, UserId>, IUserRepo
    {
        public UserRepo(UserContext context) : base(context)
        {

        }
    }
}
