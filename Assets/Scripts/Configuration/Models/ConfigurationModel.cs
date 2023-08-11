using System;

namespace Assets.Scripts.Configuration.Models
{
    [Serializable]
    public class ConfigurationModel
    {
        public string Host;
        public int Port;
        public string AuthHost;
        public string LoginToken;
        public DefaultLoginModel DefaultLogin;


        [Serializable]
        public class DefaultLoginModel
        {
            public string Username;
            public string Password;
        }
    }
}
