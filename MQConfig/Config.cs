using System;

namespace MQConfig
{
   public class Config {
        public string HostName { get; set; } = "192.168.1.162";
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string QueName { get; set; } = "Szyj.Microservice.Test.Factory.wdn";
        public string ExChangeName { get; set; } = "EquipmentEvents";
        public string RouteKey { get; set; } = "SynchPatientEvents";

        public int Port { get; set; } = 5672;
   }
}
