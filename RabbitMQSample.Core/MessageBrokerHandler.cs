using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQSample.Core;

public class MessageBrokerHandler : IMessageBrokerHandler
{

    public (IModel, IConnection) Connect()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "1234"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        return (channel, connection);
    }

    private void UseConnection(Action<IModel, IConnection> action)
    {
        var (channel, connection) = Connect();

        using (channel)
        using (connection)
        {
            action(channel, connection);
        }
    }

    public void ListenMessages(IModel channel, string queue)
    {
        ReceiveMessage(channel, queue);
    }

    public void Receive(string queue)
    {
        UseConnection((channel, _) => 
        {
            ReceiveMessage(channel, queue);
        });
    }

    public void ReceiveMessage(IModel channel, string queue)
    {
        channel.QueueDeclare(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[x] Received {message} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            Thread.Sleep(3000);

            Console.WriteLine($"[x] Done - {message} - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            channel.BasicAck(
                deliveryTag: ea.DeliveryTag,
                multiple: false
            );
        };

        channel.BasicConsume(
            queue: queue,
            autoAck: false,
            consumer: consumer
        );
    }

    public void Send(string queue, string message)
    {
        UseConnection((channel, _) =>
        {
            channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queue,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[x] Sent {message}");
        });
    }
}
