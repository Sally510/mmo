using Assets.Scripts.Client;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    public GameObject Panel;
    public InventoryManager inventoryManager;

    public void TogglePanel()
    {
        if (Panel != null)
        {
            bool newVisiblity = !Panel.activeSelf;
            Panel.SetActive(newVisiblity);

            //if we're closing the inventory window, we have to commit the inventory changes
            if (!newVisiblity)
            {
                inventoryManager.CommitChanges();
            }
        }
    }

    public void AddChange(byte slot1, byte slot2)
    {
        inventoryManager.AddChange(slot1, slot2);
    }
}
