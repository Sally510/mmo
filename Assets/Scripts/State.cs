using Assets.Scripts.Client.Models;
using System.Collections.Generic;
using static Assets.Scripts.Client.Models.EquippedItemListModel;

namespace Assets.Scripts
{
    public static class State
    {
        public static List<CharacterOptionModel> CharacterOptions { get; set; } = new();

        public static (CharacterModel character, CharacterOptionModel option) LoggedCharacter { get; set; }
        public static List<EquippedItemModel> EquippedItems { get; set; }
        public static List<InventoryItemModel> InventoryItems { get; set; }

        public static void Clear()
        {
            CharacterOptions = new();
            InventoryItems = null;
            EquippedItems = null;
        }
    }
}