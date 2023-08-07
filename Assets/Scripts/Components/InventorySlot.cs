using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public InventoryPage inventoryPage;

    public byte slot;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        InventoryItem inventoryItem = dropped.GetComponent<InventoryItem>();
        inventoryItem.destinationInventorySlot = this;

        //if theres an item inside the slot we swap it
        InventoryItem currentInventoryItem = gameObject.GetComponentInChildren<InventoryItem>();
        if (currentInventoryItem != null)
        {
            currentInventoryItem.transform.SetParent(inventoryItem.sourceInventorySlot.transform);
        }

        inventoryPage.AddChange(slot, inventoryItem.sourceInventorySlot.slot);
    }
}
