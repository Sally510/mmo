using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ConfigurationManager
    {
        private static ConfigurationModel _config; 
        public static ConfigurationModel Config
        {
            get
            {
                if(_config == null)
                {
                    TextAsset asset = (TextAsset)Resources.Load("config", typeof(TextAsset));
                    _config = JsonUtility.FromJson<ConfigurationModel>(asset.text);
                }

                return _config;
            }
        }

        public static void Clear()
        {
            _config = null;
        }

        [Serializable]
        public class ConfigurationModel
        {
            public string Host;
            public int Port;
            public string AuthHost;
            public DefaultLoginModel DefaultLogin;


            [Serializable]
            public class DefaultLoginModel
            {
                public string Username;
                public string Password;
            }
        }
    }
}
