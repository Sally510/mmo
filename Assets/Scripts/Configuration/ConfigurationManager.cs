using Assets.Scripts.Configuration.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Configuration
{
    public static class ConfigurationManager
    {
        private static ConfigurationModel _config;
        public static ConfigurationModel Config
        {
            get
            {
                if (_config == null)
                {
                    TextAsset asset = (TextAsset)Resources.Load("config", typeof(TextAsset));
                    _config = JsonUtility.FromJson<ConfigurationModel>(asset.text);
                }

                return _config;
            }
        }

        private static Dictionary<long, ItemModel> _items;
        public static Dictionary<long, ItemModel> ItemMap
        {
            get
            {
                if (_items == null)
                {
                    TextAsset asset = (TextAsset)Resources.Load("items", typeof(TextAsset));
                    _items = JsonUtility
                        .FromJson<ItemListModel>(asset.text)
                        .Items
                        .ToDictionary(x => x.Id, x => x);
                }

                return _items;
            }
        }

        public static void Clear()
        {
            _config = null;
            _items = null;
        }


    }
}
