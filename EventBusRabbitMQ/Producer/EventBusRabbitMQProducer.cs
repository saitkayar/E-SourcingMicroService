using EventBusRabbitMQ.Events.Intefaces;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly int _retryCount;

        public EventBusRabbitMQProducer(IRabbitMQPersistentConnection persistentConnection, int retryCount = 5)
        {
            _persistentConnection = persistentConnection;
            _retryCount = retryCount;
        }

        public void Publish(string queueName, IEvent @event)
        {
            if (!_persistentConnection.IsConnected) {_persistentConnection.TryConnect();}
            var policy = RetryPolicy.Handle<SocketException>()
               .Or<BrokerUnreachableException>()
               .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            using (var channel=_persistentConnection.CreateModel())
            {
                channel.QueueDeclare(queueName, durable:false, exclusive:false,autoDelete:false,arguments:null);
                var message= JsonConvert.SerializeObject(@event);
                var body=Encoding.UTF8.GetBytes(message);
                policy.Execute(() =>
                {
                    IBasicProperties properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.DeliveryMode = 2;

                    channel.ConfirmSelect();
                    channel.BasicPublish(
                        exchange: "",
                        routingKey: queueName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                    channel.WaitForConfirmsOrDie();

                    channel.BasicAcks += (sender, eventArgs) =>
                    {
                        Console.WriteLine("Sent RabbitMQ");
                        //implement ack handle
                    };
                });

            }

        }
    }
}
