using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.ComponentModel.Design;
using System;

namespace CommandService.Controllers
{
    [Route("/api/c/Platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommansdForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform {platformId}");

            if (!_repository.PlatformExits(platformId)) { return NotFound(); }

            var getcommandsforplatform = _repository.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(getcommandsforplatform));
        }

        [HttpGet("{commandId}" , Name="GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId) 
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform {platformId} / {commandId}");

            if (!_repository.PlatformExits(platformId)) { return NotFound(); }

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null) { return NotFound(); }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto command)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform {platformId}");

            if (command == null) { throw new ArgumentNullException(); }

            if (!_repository.PlatformExits(platformId)) {  return NotFound(); }

            var commandid = _mapper.Map<Command>(command);

            _repository.CreateCommand(platformId, commandid);
            _repository.SaveChanges();

            var readDto = _mapper.Map<CommandReadDto>(commandid);

            return CreatedAtRoute("GetCommandForPlatform", new {platformId = platformId, commandid = readDto.Id}, readDto);
        }
    }
}
