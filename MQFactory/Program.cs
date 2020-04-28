using System;
using System.Text;
using MQConfig;
using RabbitMQ.Client;

//MQ生产者
namespace MQFactory {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("MQ生产者 starting!");
            //StartFactoryByDirect (new Config ());

            //Fanout方式,广播模式
            StartFactoryByDFanout (new Config { ExChangeName = "fanoutExChange", QueName = "fanoutQN" });
        }

        #region Fanout方式,广播模式
        static void StartFactoryByDFanout (Config config) {
            //创建工厂
            ConnectionFactory factory = new ConnectionFactory {
                UserName = config.UserName,
                Password = config.Password,
                HostName = config.HostName
            };

            //创建链接
            var connection = factory.CreateConnection ();

            //创建通道
            var channel = connection.CreateModel ();

            //定义模式
            var extype = ExchangeType.Fanout;

            Console.WriteLine ("==>ExchangeType:" + extype + "\n");

            //定义交换机
            channel.ExchangeDeclareNoWait (
                config.ExChangeName,
                type : extype,
                durable : true,
                autoDelete : false);

            //定义队列
            channel.QueueDeclare (config.QueName, durable : true, exclusive : false, autoDelete : false);

            //交换机和队列绑定
            channel.QueueBind (config.QueName, config.ExChangeName, "");

            Console.WriteLine ("==>广播不需要写queue，routing_key为空\n");

            Console.WriteLine ("==>\nRabbitMQ连接成功，请输入消息，输入exit退出！\n");

            string input = "";

            do {
                input = Console.ReadLine ();

                var sendBytes = Encoding.UTF8.GetBytes (input);

                //发布消息
                channel.BasicPublish (config.ExChangeName, "", null, sendBytes);

            } while (input?.Trim ().ToLower () != "exit");

            channel.Close ();

            connection.Close ();
        }
        #endregion

        #region Direct方式
        static void StartFactoryByDirect (Config config) {
            //创建工厂
            ConnectionFactory factory = new ConnectionFactory {
                UserName = config.UserName,
                Password = config.Password,
                HostName = config.HostName
            };

            //创建链接
            var connection = factory.CreateConnection ();

            //创建通道
            var channel = connection.CreateModel ();

            //定义交换机
            var extype = ExchangeType.Direct;
            Console.WriteLine ("==>ExchangeDeclare");

            if (extype == ExchangeType.Fanout) {
                //channel.ExchangeDeclare (exChangeName, extype);
                channel.ExchangeDeclare (config.ExChangeName, "fanout");
            } else {
                channel.ExchangeDeclare (config.ExChangeName, extype, false, false, null);
            }

            Console.WriteLine ("==>QueueDeclare");

            //声明队列
            //生产者将消息投递到Queue中，实际上这在RabbitMQ中这种事情永远都不会发生
            //将队列绑定到交换机
            if (extype == ExchangeType.Fanout) {
                channel.QueueDeclare (config.QueName);
                channel.QueueBind (config.QueName, config.ExChangeName, "", null);
            } else {
                channel.QueueDeclare (config.QueName, false, false, false, null);
                channel.QueueBind (config.QueName, config.ExChangeName, config.RouteKey, null);
            }

            Console.WriteLine ("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input = "";

            do {
                input = Console.ReadLine ();

                var sendBytes = Encoding.UTF8.GetBytes (input);

                //发布消息
                if (extype == ExchangeType.Fanout) {
                    channel.BasicPublish (config.ExChangeName, "", null, sendBytes);
                } else
                    channel.BasicPublish (config.ExChangeName, config.RouteKey, null, sendBytes);

            } while (input?.Trim ().ToLower () != "exit");

            channel.Close ();

            connection.Close ();
        }
        #endregion
    }
}