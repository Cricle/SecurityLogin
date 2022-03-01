using Microsoft.AspNetCore.Mvc;
using SecurityLogin.AspNetCore.Services;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController:ControllerBase
    {
        private readonly LoginService loginService;

        public LoginController(LoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FlushKey()
        {
            var res = await loginService.FlushRSAKeyAsync();
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Regist(string connectId,string userName,string password)
        {
            var res = await loginService.RegistAsync(connectId,userName,password);
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Login(string connectId, string userName, string password)
        {
            var res = await loginService.LoginAsync(connectId, userName, password);
            return Ok(res);
        }
    }
}
