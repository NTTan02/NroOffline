using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    private ItemData itemData;
    private int itemIndex;
    private InventoryUI inventoryUI;

    public void Setup(ItemData data, int index, InventoryUI ui)
    {
        itemData     = data;
        itemIndex    = index;
        inventoryUI  = ui;

        if (itemIcon != null)
        {
            itemIcon.sprite = data.icon;
            itemIcon.color  = data.icon != null ? Color.white : new Color(1,1,1,0.2f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Click vào slot → hiện popup
        inventoryUI?.ShowItemInfo(itemData, itemIndex);
    }
}