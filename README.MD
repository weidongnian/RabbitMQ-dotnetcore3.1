RabbitMQ+dotnet core的简单应用

1，创建空白解决方案，创建生产者控制台项目,引用RabbitMQ.Client库

    dotnet new console -n MQFactory
    
    dotnet add package RabbitMQ.Client
    
2，在生产者控制台项目里，完善相关代码，请先安装好rabbitmq,并设置好用户、开放端口

           ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "192.*.*.*"
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
        
        
3，创建消费者控制台项目

    dotnet new console -name MQConsumer
    
    dotnet package add RabbitMQ.Client
    
4，完善消费者项目代码

           var factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "192.*.*.*"
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
            
5，运行生产者项目和消费者项目，在生产者控制台输入内容，没有问题，可以在消费者控制台那边看到输出内容了

6，RabbitMQ提供了四种Exchange模式：direct,fanout,topic,header 

7，topic 可以做一对一，一对多的即时通讯和聊天，routekey=用户的唯一标识

10,客户端如何保护密码的问题