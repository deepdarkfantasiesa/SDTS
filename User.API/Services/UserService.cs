using Grpc.Core;
using MediatR;
using User.Infrastructure.Repositories;
using user_rpcservices;

namespace User.API.Services
{
    public class UserService:UserGrpc.UserGrpcBase
    {
        private readonly IMediator _mediator;
        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<CreateUserResult> CreateUser(CreateUserCommand command,ServerCallContext context)
        {
            await _mediator.Send(new User.API.Application.Commands.CreateUserCommand(command.UserName));
            return new CreateUserResult();
        }
    }
}
