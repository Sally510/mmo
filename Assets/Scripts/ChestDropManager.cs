using Assets.Scripts.Client;
using Assets.Scripts.Client.Models;
using Assets.Scripts.Client.Types;
using Assets.Scripts.Configuration;
using Assets.Scripts.Helpers;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class ChestDropManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject chestPrefab;
        [SerializeField]
        private GameObject inventoryItemPrefab;

        [SerializeField]
        private GameObject treasureChest;
        public InventorySlot[] chestSlots;

        private void PacketEventHandler_ChestDropEvent(object sender, ChestDropModel e)
        {
            Vector3 worldPosition = e.IsoPosition.FromIsoToWorld();
            GameObject newChest = Instantiate(chestPrefab, worldPosition, Quaternion.identity);
            newChest.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => OpenChest(e.DroppedChestId));
            Debug.Log("New chest drop");
        }

        async void OpenChest(long droppedChestId)
        {
            await ClientManager.OpenChestAsync(droppedChestId, destroyCancellationToken);
        }

        private void PacketEventHandler_ChestItemsEvent(object sender, ChestItemsModel e)
        {
            foreach(InventorySlot slot in chestSlots)
            {
                InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
                if(item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            int i = 0;
            foreach(ChestItemsModel.DroppedItemModel item in e.Items)
            {
                InventorySlot slot = chestSlots[i++];
                if(ConfigurationManager.ItemMap.TryGetValue(item.ItemId, out var itemModel))
                {
                    InventoryItem inventoryItem = InventoryItem.InstantiateNewItem(inventoryItemPrefab, slot, itemModel.Name, item.Quantity, item.DroppedItemId);

                    inventoryItem.OnClick += InventoryItem_OnClick;
                }
            }

            treasureChest.SetActive(true);
        }

        private async void InventoryItem_OnClick(object sender, UnityEngine.EventSystems.PointerEventData e)
        {
            if(sender is InventoryItem inventoryItem && inventoryItem.Data is long droppedItemid)
            {
                PickupStatusType status = await ClientManager.PickupItemAsync(droppedItemid, destroyCancellationToken);
                Debug.Log(status);
                if(status == PickupStatusType.Ok)
                {
                    Destroy(inventoryItem.gameObject);
                }
            }
        }

        void OnEnable()
        {
            PacketEventHandler.ChestDropEvent += PacketEventHandler_ChestDropEvent;
            PacketEventHandler.ChestItemsEvent += PacketEventHandler_ChestItemsEvent;
        }

        void OnDisable()
        {
            PacketEventHandler.ChestDropEvent -= PacketEventHandler_ChestDropEvent;
            PacketEventHandler.ChestItemsEvent -= PacketEventHandler_ChestItemsEvent;
        }
    }
}