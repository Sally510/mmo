using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler ,IEndDragHandler  
{
    public Image image;
    public TMP_Text itemName;
    public TMP_Text itemQuantity;
    [HideInInspector] public Transform parentAfterDrag;

    public void InitializeItem(string name, int quantity)
    {
        itemName.text = name;
        itemQuantity.text = quantity.ToString();

    }

    // Drag and drop
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
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
        transform.SetParent(parentAfterDrag);
    }






}
