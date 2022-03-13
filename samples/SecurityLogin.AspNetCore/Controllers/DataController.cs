using Microsoft.AspNetCore.Mvc;
using SecurityLogin.AspNetCore.Services;
using SecurityLogin.Redis.Finders;
using SecurityLogin.Redis.Finders;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController:ControllerBase
    {
        private readonly StudentCacheFinder studentCacheFinder;
        private readonly StudentIdCacheFinder studentIdCacheFinder;
        private readonly MessagePackCacheFinder messagePackCacheFinder;

        public DataController(StudentCacheFinder studentCacheFinder,
            StudentIdCacheFinder studentIdCacheFinder,
            MessagePackCacheFinder messagePackCacheFinder)
        {
            this.messagePackCacheFinder = messagePackCacheFinder;
            this.studentIdCacheFinder= studentIdCacheFinder;
            this.studentCacheFinder = studentCacheFinder;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Get()
        {
            var sw = Stopwatch.GetTimestamp();
            var res = await studentCacheFinder.FindAsync("aaa");
            var ed = new TimeSpan(Stopwatch.GetTimestamp() - sw);
            var size =await studentCacheFinder.GetSizeAsync("aaa");
            return Ok(new
            {
                //res,
                size= size,
                Elase = ed
            });
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMP()
        {
            var sw = Stopwatch.GetTimestamp();
            var res = await messagePackCacheFinder.FindAsync("bbb");
            var ed = new TimeSpan(Stopwatch.GetTimestamp() - sw);
            var size = await messagePackCacheFinder.GetSizeAsync("bbb");
            return Ok(new
            {
                //res,
                size = size,
                Elase = ed
            });
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetId()
        {
            var res = await studentIdCacheFinder.FindAsync("aaa");
            return Ok(res);
        }
    }
}
