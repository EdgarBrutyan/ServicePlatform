using PlatformService.Dtos;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace PlatformService.SyncDataService.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _env = env;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8, 
                "application/json"
            );

            string? connection = _env.IsDevelopment() ? _configuration["CommandServiceDev"] : _configuration["CommandServiceProduction"]; 

            var responce = await _httpClient.PostAsync($"{connection}", httpContent);

            if(responce.IsSuccessStatusCode)
            {
                Console.WriteLine("The POSTING was OK");
            }
            else
            {
                Console.WriteLine("The POSTING was not OK");
            }
        }
    }
}
