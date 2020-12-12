using System;

namespace MQConfig
{
   public class Config {
        public string HostName { get; set; } = "127.0.0.1";
        public string UserName { get; set; } = "yjian";
        public string Password { get; set; } = "yjian";
        public string QueName { get; set; } = "Factory";
        public string ExChangeName { get; set; } = "EquipmentEvents";
        public string RouteKey { get; set; } = "chat";
    }
}
