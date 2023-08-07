using Assets.Scripts;
using Assets.Scripts.Configuration;
using Assets.Scripts.Configuration.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    
    public void AddItem(string name, int quantity)
    {
        // Find any empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(slot, name, quantity);
                return;
            }
        }
    }

    void SpawnNewItem(InventorySlot slot, string name, int quantity)
    {
        
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        newItem.GetComponent<InventoryItem>().itemName.text = name;
        newItem.GetComponent<InventoryItem>().itemQuantity.text = FormatQuantity(quantity);
    }

    private string FormatQuantity(int quantity)
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
        foreach(var inventoryItem in State.InventoryItems) 
        {
            if(ConfigurationManager.ItemMap.TryGetValue(inventoryItem.ItemId, out ItemModel item))
            {
                AddItem(item.Name,inventoryItem.Quantity);
            }
        }
    }
}
