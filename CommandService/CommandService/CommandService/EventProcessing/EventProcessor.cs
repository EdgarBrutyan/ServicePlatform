using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopefactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopefactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType) 
            {
                case EventType.PlatformPublished:
                    addPlatform(message);
                    break;
                default: 
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event) 
            {
                case "PlatformPublished":
                    Console.WriteLine("Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine the event type");
                    return EventType.Undetermined;
            }
        }


        private void addPlatform(string platformPublished) 
        {
            using (var scope = _scopefactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublished);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformIdExists(plat.ExternalID)) 
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("The Platform is added");
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not add Platform to DB: {ex.Message}");
                }
            }
        }
    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
