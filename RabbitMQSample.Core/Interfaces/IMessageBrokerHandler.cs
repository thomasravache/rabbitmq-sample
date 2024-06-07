using RabbitMQ.Client;

namespace RabbitMQSample.Core;

public interface IMessageBrokerHandler
{
    void Send(string queue, string message);
    void Receive(string queue);
}