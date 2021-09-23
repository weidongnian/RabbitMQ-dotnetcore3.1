using System;
using System.Collections.Generic;
using System.Text;
using MQConfig;
using RabbitMQ.Client;

//MQ生产者
namespace MQFactory {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("MQ生产者 starting!");

            //不用定义队列
            // StartFactory (new Config {
            //         ExChangeName = "Fanout",
            //             QueName = "dotnetQn",
            //             RouteKey = "websocker"
            //     },
            //     ExchangeType.Fanout);

            // StartFactory (new Config {
            //         ExChangeName = "EquipmentEvents",
            //             QueName = "dotnetQn",
            //             RouteKey = "websocker"
            //     },
            //     ExchangeType.Direct);

            //topic,可以做一对一，一对多的即时通讯和聊天，routekey=用户的唯一标识
            StartFactory (new Config {
                    ExChangeName = "amq.direct", QueName = "wdn.factory"
                },
                ExchangeType.Direct);

            //Fanout方式,广播模式
            // StartFactoryByDFanout (new Config {
            //     ExChangeName = "EquipmentEvents",
            //         QueName = "dotnetQn",
            //         RouteKey = "websocker"
            // });
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

        #region Direct方式,不用定义队列
        static void StartFactory (Config config, string exchangeType = "direct") {
            //创建工厂
           ConnectionFactory factory = new ConnectionFactory {
            UserName = config.UserName,
            Password = config.Password,
            HostName = config.HostName
            };

            //创建链接
            using var connection = factory.CreateConnection ();

            //创建通道
            using var channel = connection.CreateModel ();

            //定义交换机
            Console.WriteLine ("==>ExchangeDeclare," + exchangeType);
            
            channel.ExchangeDeclare(config.ExChangeName, exchangeType,
                true,
                false,
                null);
            Console.WriteLine ("\nRabbitMQ连接成功，请输入消息，输入exit退出！");

            string input = "";
            do {
                input = Console.ReadLine ();
                var sendBytes = Encoding.UTF8.GetBytes (input);
                //发布消息
                string routeKey = config.RouteKey;
                channel.BasicPublish (config.ExChangeName, routeKey, null, sendBytes);
            } while (input?.Trim ().ToLower () != "exit");
        }
        
        #endregion
    }
}