using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    public GameObject Panel;

    [SerializeField]
    private GameObject _itemPrefab;

    [SerializeField]
    private GameObject _itemParent;

    public void TogglePanel()
    {
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
