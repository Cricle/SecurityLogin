using Microsoft.AspNetCore.Mvc;
using SecurityLogin.AspNetCore.Services;
using SecurityLogin.Redis.Finders;
using System.Threading.Tasks;

namespace SecurityLogin.AspNetCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController:ControllerBase
    {
        private readonly StudentCacheFinder studentCacheFinder;
        private readonly StudentIdCacheFinder studentIdCacheFinder;

        public DataController(StudentCacheFinder studentCacheFinder,
            StudentIdCacheFinder studentIdCacheFinder)
        {
            this.studentIdCacheFinder= studentIdCacheFinder;
            this.studentCacheFinder = studentCacheFinder;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Get()
        {
            var res = await studentCacheFinder.FindAsync("aaa");
            return Ok(res);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetId()
        {
            var res = await studentIdCacheFinder.FindAsync("aaa");
            return Ok(res);
        }
    }
}
