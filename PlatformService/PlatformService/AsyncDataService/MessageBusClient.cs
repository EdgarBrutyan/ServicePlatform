using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;

            string RabbitMQHost = _env.IsDevelopment() ? _configuration["RabbitMQHostDev"] : _configuration["RabbitMQHostPro"];
            string RabbitMQPort = _env.IsDevelopment() ? _configuration["RabbitMQPortDev"] : _configuration["RabbitMQPortPro"];

            var factory = new ConnectionFactory() { HostName = RabbitMQHost, Port = int.Parse(RabbitMQPort) };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnetionShutdown;

                Console.WriteLine("Connected to MessageBus");
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"--> Could not connect to message bus because: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnetionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Connection Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platform)
        {
            var message = JsonSerializer.Serialize(platform);

            if(_connection.IsOpen)
            { 
                Console.WriteLine("--> RabbitMQ Connection Open, sending message");
                SendMessage(message);
            }

            else
            {
                Console.WriteLine("RabbitMQ Connection is closed, not sending");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
