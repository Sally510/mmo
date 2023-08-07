using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    [HideInInspector] public InventorySlot destinationInventorySlot;
    [HideInInspector] public InventorySlot sourceInventorySlot;

    public void InitializeItem(string name, int quantity)
    {
        itemName.text = name;
        itemQuantity.text = quantity.ToString();
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
}
