using Domain.Abstraction;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure.Repositories;

namespace User.API.Application.Commands
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepo _repo;

        public CreateUserCommandHandler(IUserRepo userRepo)
        {
            _repo = userRepo;
        }
        public async Task<bool> Handle(CreateUserCommand user, CancellationToken cancellationToken)
        {
            var address = new Address("1", "2", "3", "4", "5");
            var user1 = new Users(address, user.UserName);

            await _repo.AddAsync(user1, cancellationToken);
            return true;
        }
    }
}
