using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SanalPos.API.Controllers;
using SanalPos.Application.Authentication.Commands.Login;
using SanalPos.Application.Authentication.Commands.Register;
using SanalPos.Application.Authentication.Common;

namespace SanalPos.API.Controllers
{
    public class AuthenticationController : BaseApiController
    {
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResult>> Login(LoginCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResult>> Register(RegisterCommand command)
        {
            return await Mediator.Send(command);
        }
    }
} 