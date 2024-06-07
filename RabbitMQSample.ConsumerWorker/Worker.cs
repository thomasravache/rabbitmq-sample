using RabbitMQ.Client;
using RabbitMQSample.Core;

namespace RabbitMQSample.ConsumerWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly MessageBrokerHandler _messageBrokerHandler;

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _messageBrokerHandler = new MessageBrokerHandler();

        var (channel, connection) = _messageBrokerHandler.Connect();

        _channel = channel;
        _connection = connection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        _messageBrokerHandler.ListenMessages(_channel, MessagingQueues.HelloWorld);

        while (!stoppingToken.IsCancellationRequested)
        {
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
        base.Dispose();
    }
}
