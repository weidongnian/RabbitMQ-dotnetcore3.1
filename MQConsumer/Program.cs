using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//队列的消费者
namespace MQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("队列的消费者 starting!");
            Start();
        }

        static void Start()
        {
            var factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "*.*.*.*"
            };

            var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            
            //事件基本消费者
            EventingBasicConsumer ebConsumer=new EventingBasicConsumer(channel);
            
            //接收到消息事件
            ebConsumer.Received += (ch,ea) =>
            {

                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("收到消息了："+message);
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag,false);
            };
            
            //启动消费者 设置为手动应答消息
            channel.BasicConsume("hello", false, ebConsumer);
            
            Console.Write("消费者已经开启");
            Console.ReadKey();
            channel.Close();
            connection.Close();
        }
    }
}