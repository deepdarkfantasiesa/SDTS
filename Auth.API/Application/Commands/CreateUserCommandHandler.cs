using Auth.Domain.AggregatesModel.UserAggregate;
using Auth.Infrastructure.Repositories;
using Domain.Abstraction;

namespace Auth.API.Application.Commands
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
            var user1 = new Domain.AggregatesModel.UserAggregate.User(new List<Address>() { address }, user.UserName);

            await _repo.AddAsync(user1, cancellationToken);
            return true;
        }
    }
}
