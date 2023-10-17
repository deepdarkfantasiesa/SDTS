using Domain.Abstraction;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.AggregatesModel.UserAggregate;

namespace User.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public UserRepository(UserContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _context.GetCurrentTransaction().GetDbTransaction();
        }

        public Users Add(Users user)
        {
            return _context.Users.Add(user).Entity;
        }

        public virtual Task<Users> AddAsync(Users user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(user));
        }

        public virtual bool Remove(Entity entity)
        {
            _context.Remove(entity);
            return true;
        }

        public Task<bool> RemoveAsync(Entity entity)
        {
            return Task.FromResult(Remove(entity));
        }

        public Users Update(Users user)
        {
            return _context.Users.Update(user).Entity;
        }

        public Task<Users> UpdateAsync(Users user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(user));
        }

        public virtual async Task<Users> GetAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
