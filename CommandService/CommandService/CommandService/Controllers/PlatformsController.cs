using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper) 
        { 
            _repository = repository;
            _mapper =  mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms() 
        {
            Console.WriteLine("Getting Platforms");

            var plaformItem = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(plaformItem));
        }

        [HttpPost]
        public ActionResult Connection()
        {
            Console.WriteLine("The platform is accepted");
            return Ok("The command service connects to PlatformService");
        }
    }
}
