using Infrastructure.Core;
using MediatR;
using User.Domain.AggregatesModel.UserAggregate;
using User.Infrastructure.Repositories;

namespace User.API.Application.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,bool>
    {
        private readonly IUserRepository _repository;
        public CreateUserCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(CreateUserCommand user, CancellationToken cancellationToken)
        {
            var address = new Address("1", "2", "3", "4", "5");
            var user1 = new Users(address, user.UserName);

            _repository.Add(user1);
            await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return true;
        }
    }
}
