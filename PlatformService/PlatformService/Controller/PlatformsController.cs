using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
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
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messagebusclient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataclient = commandDataClient;
            _messageBusClient = messagebusclient;
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

            // Send Sync Message

            try
            {
                await _commandDataclient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Send Async Message

            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "PlatformPublished";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Could not send async: {ex.Message}");
            }

            return CreatedAtRoute("GetPlatformById", new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
