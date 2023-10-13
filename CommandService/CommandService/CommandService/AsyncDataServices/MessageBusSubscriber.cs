using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private readonly IWebHostEnvironment _env;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(
            IConfiguration configuration, 
            IEventProcessor eventProcessor, 
            IWebHostEnvironment env)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            _env = env;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            string _Host = (_env.IsDevelopment()) ? _configuration["RabbitMQHostDev"] : _configuration["RabbitMQHostPro"];
            string _Port = (_env.IsDevelopment()) ? _configuration["RabbitMQPortDev"] : _configuration["RabbitMQPortPro"];

            var factory = new ConnectionFactory() { HostName = _Host, Port = int.Parse(_Port) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");

            Console.WriteLine("--> Listening on the Message Bus");
            _connection.ConnectionShutdown += RabbitMQ_ConnetionShutdown;
        }

        private void RabbitMQ_ConnetionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connetion Shutdown");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                Console.WriteLine("Message Received!");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
