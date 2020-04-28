using System;

namespace MQConfig
{
   public class Config {
        public string HostName { get; set; } = "*.*.240.230";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string QueName { get; set; } = "hello";
        public string ExChangeName { get; set; } = "exHello";
        public string RouteKey { get; set; } = "hello";
    }
}
