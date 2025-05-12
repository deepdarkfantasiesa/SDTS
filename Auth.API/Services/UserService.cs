using Grpc.Core;
using MediatR;
using user_rpcservices;

namespace Auth.API.Services
{
    public class UserService : UserGrpc.UserGrpcBase
    {
        private readonly IMediator _mediator;
        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task<CreateUserResult> CreateUser(CreateUserCommand command, ServerCallContext context)
        {
            //await _mediator.Send(new CreateUserCommand(command.UserName));
            //return new CreateUserResult();
            return null;
        }
    }
}
