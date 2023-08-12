using Assets.Scripts;
using Assets.Scripts.Client;
using Assets.Scripts.Configuration;
using Assets.Scripts.Configuration.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private readonly Stack<(byte, byte)> _slotChanges = new();
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    private void PacketEventHandler_NewInventoryItemEvent(object sender, Assets.Scripts.Client.Models.InventoryItemModel e)
    {
        throw new System.NotImplementedException();
    }

    public void AddItem(int slot, string name, int quantity)
    {
        InventorySlot inventorySlot = inventorySlots[slot];
        InventoryItem itemInSlot = inventorySlot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot == null)
        {
            SpawnNewItem(inventorySlot, name, quantity);
        }
    }

    void SpawnNewItem(InventorySlot slot, string name, int quantity)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        newItem.GetComponent<InventoryItem>().itemName.text = name;
        newItem.GetComponent<InventoryItem>().itemQuantity.text = FormatQuantity(quantity);
    }

    //TODO: add this to a helper class
    public static string FormatQuantity(int quantity)
    {
        if (quantity >= 1000000)
        {
            double millions = (double)quantity / 1000000.0;
            return millions.ToString("0.#") + "M";
        }
        else if (quantity >= 1000)
        {
            double thousands = (double)quantity / 1000.0;
            return thousands.ToString("0.#") + "K";
        }
        else
        {
            return quantity.ToString();
        }
    }

    public void Start()
    {
        foreach (var inventoryItem in State.InventoryItems)
        {
            if (ConfigurationManager.ItemMap.TryGetValue(inventoryItem.ItemId, out ItemModel item))
            {
                AddItem(inventoryItem.Slot, item.Name, inventoryItem.Quantity);
            }
        }
    }

    public void AddChange(byte slot1, byte slot2)
    {
        //swaping with itself makes no sense
        if (slot1 == slot2)
        {
            return;
        }

        //if the last added slots are the same as the current values
        //we pop it because its negating itself
        if (_slotChanges.TryPeek(out var change) && (change == (slot1, slot2) || change == (slot2, slot1)))
        {
            _slotChanges.Pop();
        }

        _slotChanges.Push((slot1, slot2));
    }

    public async void CommitChanges()
    {
        if (_slotChanges.Count > 0)
        {
            try
            {
                bool ok = await ClientManager.CommitInventoryStateAsync(_slotChanges.ToList(), destroyCancellationToken);
                if (ok)
                {
                    Debug.Log("Successfully saved the inventory changes.");
                }
                else
                {
                    Debug.Log("Failed saving inventory changes.");

                    //sync the items from the server
                    //TODO: update the positions..
                    State.InventoryItems = (await ClientManager.GetInventoryItemsAsync(destroyCancellationToken)).Items;
                }
            }
            finally
            {
                _slotChanges.Clear();
            }
        }
    }

    void OnEnable()
    {
        PacketEventHandler.NewInventoryItemEvent += PacketEventHandler_NewInventoryItemEvent;
    }

    void OnDisable()
    {
        PacketEventHandler.NewInventoryItemEvent += PacketEventHandler_NewInventoryItemEvent;
    }
}
