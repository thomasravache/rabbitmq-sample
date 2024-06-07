using Microsoft.Extensions.DependencyInjection;
using RabbitMQSample.Core;

var services = new ServiceCollection();

// add services

services.AddTransient<IMessageBrokerHandler, MessageBrokerHandler>();

var messageBroker = new MessageBrokerHandler();

messageBroker.Receive(MessagingQueues.HelloWorld);
Console.ReadLine();