using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataService.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8, 
                "application/json"
            );

            var responce = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

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
