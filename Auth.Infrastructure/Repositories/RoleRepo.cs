using Auth.Domain.AggregatesModel.RoleAggregate;
using Infrastructure.Core;

namespace Auth.Infrastructure.Repositories
{
    public interface IRoleRepo : IRepository<Role, RoleId>
    {

    }

    public class RoleRepo : Repository<UserContext, Role, RoleId>, IRoleRepo
    {
        public RoleRepo(UserContext context) : base(context)
        {
            
        }
    }
}
