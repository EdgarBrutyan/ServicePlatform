using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        public PlatformsController() { }

        [HttpPost]
        public ActionResult Connection()
        {
            Console.WriteLine("The platform is accepted");
            return Ok("The command service connects to PlatformService");
        }
    }
}
