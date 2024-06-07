using Microsoft.Extensions.DependencyInjection;
using RabbitMQSample.Core;

var services = new ServiceCollection();

// add services

services.AddTransient<IMessageBrokerHandler, MessageBrokerHandler>();

var messageBrokerHandler = new MessageBrokerHandler();

messageBrokerHandler.Send(MessagingQueues.HelloWorld, "fala galerinha do zap");