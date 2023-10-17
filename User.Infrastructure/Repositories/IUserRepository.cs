using Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure.Repositories
{
    public interface IUserRepository:IRepository<Users>
    {
    }
}
