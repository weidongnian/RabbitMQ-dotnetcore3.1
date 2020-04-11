using System;
using RabbitMQ.Client;
using  System.Text;

//MQ生产者
namespace MQFactory
{
    class Program
    {
        private const string queName = "hello";
        private const string exChangeName = "exHello";
        private const string routeKey = "hello";
        
        static void Main(string[] args)
        {
            Console.WriteLine("MQ生产者 starting!");
            StartFactory();
        }

        static void StartFactory()
        {
            //创建工厂
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "*.*.*.*"
            };
            
            //创建链接
            var connection = factory.CreateConnection();
            
            //创建通道
            var channel= connection.CreateModel();
            
            //定义交换机
            channel.ExchangeDeclare(exChangeName,ExchangeType.Direct,false,false,null);
            
            //声明队列
            //生产者将消息投递到Queue中，实际上这在RabbitMQ中这种事情永远都不会发生
            channel.QueueDeclare(queName,false,false,false,null);
            
            //将队列绑定到交换机
            channel.QueueBind(queName,exChangeName, routeKey, null);
            
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入exit退出！");
            
            string input="";
            
            do
            {
                input = Console.ReadLine();
                
                var sendBytes = Encoding.UTF8.GetBytes(input);
                
                //发布消息
                channel.BasicPublish(exChangeName, routeKey, null, sendBytes);
                
            } while (input?.Trim().ToLower() != "exit");

            channel.Close();
            
            connection.Close();
        }
    }
}