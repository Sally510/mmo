using Assets.Scripts.Client.Types;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Configuration.Models
{
    [Serializable]
    public class ItemListModel
    {
        public List<ItemModel> Items;
    }

    [Serializable]
    public class ItemModel
    {
        public long Id;
        public string Name;
        public bool IsStackable;
        public ItemType ItemType;
    }
}
