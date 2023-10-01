using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataclient;
        private readonly IPlatformRepo _repo;

        public PlatformsController(IPlatformRepo repo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataclient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatform()
        {
            Console.WriteLine("--> Getting Platforms");

            var platforms = _repo.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatforms(int id) 
        {
            Console.WriteLine("--> Getting the Platform");

            var platforms = _repo.GetPlatformById(id);

            if (platforms != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platforms));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform) 
        {
            if(platform == null)
            {
                return BadRequest();
            }

            var plat = _mapper.Map<Platform>(platform);
            _repo.CreatePlatform(plat);
            _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(plat);

            try
            {
                await _commandDataclient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return CreatedAtRoute("GetPlatformById", new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
