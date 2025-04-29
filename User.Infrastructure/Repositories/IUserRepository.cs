using Infrastructure.Core;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository<Users, UserId>
    {
    }
}
