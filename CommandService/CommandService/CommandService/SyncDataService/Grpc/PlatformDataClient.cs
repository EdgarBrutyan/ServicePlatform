using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataService.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _mapper = mapper;
            _env = env;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            string connection = _env.IsDevelopment() ? _configuration["GrpcPlatformDev"] : _configuration["GrpcPlatformPro"];
            Console.WriteLine($"--> Calling Grpc Service {connection}");

            var channel = GrpcChannel.ForAddress(connection);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var repy = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(repy.Platform);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"--> Could not call Grpc Server: {ex.Message}");
                return null;
            }
        }
    }
}
