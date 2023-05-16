using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecurityLogin.AccessSession;
using SecurityLogin.AspNetCore.Services;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService loginService;

        public LoginController(LoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FlushKey()
        {
            var res = await loginService.FlushKeyAsync();
            return Ok(new { res.Identity, res.PublicKey });
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Regist(string connectId, string userName, string password)
        {
            var res = await loginService.RegistAsync(connectId, userName, password);
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Login(string connectId, string userName, string password)
        {
            var res = await loginService.LoginAsync(connectId, userName, password);
            return Ok(res);
        }
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult Auth()
        {
            var usn = HttpContext.Features.Get<UserSnapshot>();
            return Ok(usn);
        }
    }
}
