using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public object Data { get; private set; }

    public Image image;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    [HideInInspector] public InventorySlot destinationInventorySlot;
    [HideInInspector] public InventorySlot sourceInventorySlot;

    public event EventHandler<PointerEventData> OnClick;

    public void InitializeItem(string name, int quantity, object data = null)
    {
        itemName.text = name;
        itemQuantity.text = InventoryManager.FormatQuantity(quantity);
        Data = data;
    }

    // Drag and drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        sourceInventorySlot = gameObject.GetComponentInParent<InventorySlot>();

        image.raycastTarget = false;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(destinationInventorySlot.transform);
    }

    public static InventoryItem InstantiateNewItem(GameObject inventoryItemPrefab, InventorySlot slot, string name, int quantity, object data = null)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(name, quantity, data);
        return inventoryItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this, eventData);
    }
}
