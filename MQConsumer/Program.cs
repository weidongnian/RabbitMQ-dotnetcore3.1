using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MQConfig;

//队列的消费者
namespace MQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("队列的消费者 starting!");
            StartByDirect(new Config {ExChangeName = "amq.direct", 
                RouteKey = "SlaveEvent",
                QueName = "Szyj.Microservice.EventCenter.Slave0"});
        }

        static void StartByDirect(Config config)
        {
            config.ExChangeName = "portal.pda";
            config.RouteKey = "prehospital.pda.all.routingkey.赣6666888";
            //config.RouteKey = "prehospital.pda.all.routingkey";
            //config.RouteKey = "AAAAA";
            config.RouteKey = "CCCCC";
            config.QueName = "wdn_q";//+ Guid.NewGuid().ToString();
            var factory = new ConnectionFactory
            {
                UserName = config.UserName,
                Password = config.Password,
                HostName = config.HostName,
            };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            
            var arguments = new Dictionary<string, object>() { { "x-queue-type", "classic" } };
            
            channel.QueueDeclare(queue: config.QueName, durable: true, exclusive: false, autoDelete: true, arguments: arguments);
            
            //channel.ExchangeUnbind(config.QueName, config.ExChangeName, "AAAAA");

            channel.QueueBind(config.QueName, config.ExChangeName, config.RouteKey);

            //事件基本消费者
            EventingBasicConsumer ebConsumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            ebConsumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("收到消息了：" + message);
                
                //在这里转发一下队列
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, true);
            };

            //启动消费者 设置为手动应答消息
            channel.BasicConsume(config.QueName, false, ebConsumer);

            Console.Write("消费者已经开启");
            Console.ReadKey();
            
            channel.Close();
            connection.Close();
        }
    }
}