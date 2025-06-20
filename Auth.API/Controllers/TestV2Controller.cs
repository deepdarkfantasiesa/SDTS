using Auth.API.Application.Commands.Test;
using Domain.Abstraction.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestV2Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestV2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<string> Test(CancellationToken cancellationToken=default)
        {
            var command = new TestCommand();
            var res = await _mediator.SendAsync(command, cancellationToken);
            return res;
        }
    }
}
