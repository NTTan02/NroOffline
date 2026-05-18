using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    public Image icon;
    public Image emptyOverlay;

    private ItemData currentItem;
    private EquipmentType slotType;

    public void Setup(ItemData item, EquipmentType type)
    {
        currentItem = item;
        slotType    = type;

        if (item != null)
        {
            icon.sprite  = item.icon;
            icon.color   = Color.white;
            emptyOverlay?.gameObject.SetActive(false);
        }
        else
        {
            icon.color = new Color(1,1,1,0.1f);
            emptyOverlay?.gameObject.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            EquipmentSystem.Instance?.Unequip(slotType);
        }
    }
}
