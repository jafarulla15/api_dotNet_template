using Transporter.Common.DTO;
using Transporter.Services;
using Transporter.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Transporter.API.Controllers
{
    [Route("api/Test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISystemUserService _userService;

        public TestController(ISystemUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet ("get")]
        public async Task<string> get()
        {
            return "Hello test";
        }


    }
}
