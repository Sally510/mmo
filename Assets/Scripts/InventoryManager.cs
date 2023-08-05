using Assets.Scripts;
using Assets.Scripts.Configuration;
using Assets.Scripts.Configuration.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //[SerializeField]
    //private InventoryPage _inventoryPage;
    public InventorySlot[] inventorySlots;
    
    public void AddItem(InventoryItem item)
    {
        // Find any empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return;
            }
        }
    }

    void SpawnNewItem(InventoryItem item, InventorySlot slot)
    {
        
        //GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);

    }

    public void Start()
    {
        foreach(var inventoryItem in State.InventoryItems) 
        {
            if(ConfigurationManager.ItemMap.TryGetValue(inventoryItem.ItemId, out ItemModel item))
            {
                //add item here...
            }
        }
    }
}
