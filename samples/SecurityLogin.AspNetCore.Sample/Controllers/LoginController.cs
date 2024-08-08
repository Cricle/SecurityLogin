using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityLogin.AccessSession;
using SecurityLogin.AppLogin;
using SecurityLogin.AppLogin.Models;
using SecurityLogin.AspNetCore.Services;
using SecurityLogin.Test.AspNetCore.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DataController:ControllerBase
    {
        private readonly AppDbContext dbContext;

        public DataController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetValue()
        {
            var us = HttpContext.Features.Get<UserSnapshot>();
            var val = await dbContext.ValueStores.FirstOrDefaultAsync(x => x.UserId == us.Id);
            return Ok(val?.Value);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> SetValue([FromForm] int value)
        {
            var us = HttpContext.Features.Get<UserSnapshot>();
            var val = await dbContext.ValueStores.FirstOrDefaultAsync(x => x.UserId == us.Id);
            if (val == null)
            {
                val = new ValueStore { UserId = us.Id, Value = value };
                dbContext.ValueStores.Add(val);
            }
            else
            {
                val.Value = value;
            }
            dbContext.SaveChanges();
            return Ok(val.Value);
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class AppController:ControllerBase
    {
        private readonly AppDbContext appDbContext;
        private readonly IIdentityService<AppSession, AppSession> identityService;

        public AppController(AppDbContext appDbContext, IIdentityService<AppSession, AppSession> identityService)
        {
            this.appDbContext = appDbContext;
            this.identityService = identityService;
        }

        [HttpGet("[action]")]
        public IActionResult All()
        {
            var allKeys = appDbContext.AppInfos.AsNoTracking().ToList();
            return Ok(allKeys);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Issure([FromQuery]string appKey)
        {
            var res = await identityService.IssureTokenAsync(new AppSession { AppKey = appKey });
            return Ok(res);
        }
    }
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
