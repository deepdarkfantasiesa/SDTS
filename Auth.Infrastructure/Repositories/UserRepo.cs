using Auth.Domain.AggregatesModel.UserAggregate;
using Infrastructure.Core;

namespace Auth.Infrastructure.Repositories
{
    public interface IUserRepo : IRepository<Domain.AggregatesModel.UserAggregate.User, UserId>
    {

    }

    public class UserRepo : Repository<UserContext, Domain.AggregatesModel.UserAggregate.User, UserId>, IUserRepo
    {
        public UserRepo(UserContext context) : base(context)
        {

        }
    }
}
