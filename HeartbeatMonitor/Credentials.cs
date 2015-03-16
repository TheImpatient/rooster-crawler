using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartbeatMonitor
{
    public class Credentials
    {
        public const string ConnectionString = "Server=195.8.208.128;Port=3351;Database=rooster;Uid=crawler;Pwd=g!MqEFCXbVK0P3hv^Jy5;";
        public static string GetCredentials()
        {
            return ConnectionString;
        }
    }
}
