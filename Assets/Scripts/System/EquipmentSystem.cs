using UnityEngine;
using System.Collections.Generic;

public class EquipmentSystem : MonoBehaviour
{
    public static EquipmentSystem Instance;

    public Dictionary<EquipmentType, ItemData> equipped
        = new Dictionary<EquipmentType, ItemData>();

    private PlayerStats stats;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();

        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
            equipped[type] = null;
    }

    public void Equip(ItemData item)
    {
        if (item == null || item.itemType != ItemType.Equipment) return;
        if (equipped[item.equipmentType] != null)
            Unequip(item.equipmentType);

        equipped[item.equipmentType] = item;
        ApplyStats(item, true);

        Inventory.Instance.items.Remove(item);

        FindObjectOfType<HUDManager>()?.UpdateHUD();
        FindObjectOfType<EquipmentUI>()?.Refresh();
        FindObjectOfType<InventoryUI>()?.RefreshInventory();

        Debug.Log($"Mặc: {item.itemName}");
    }

    public void Unequip(EquipmentType type)
    {
        var item = equipped[type];
        if (item == null) return;

        equipped[type] = null;
        ApplyStats(item, false);

        Inventory.Instance.AddItem(item);

        FindObjectOfType<HUDManager>()?.UpdateHUD();
        FindObjectOfType<EquipmentUI>()?.Refresh();
        FindObjectOfType<InventoryUI>()?.RefreshInventory();

        Debug.Log($"Tháo: {item.itemName}");
    }

    void ApplyStats(ItemData item, bool add)
    {
        int mult = add ? 1 : -1;
        stats.maxHP  += item.bonusHP  * mult;
        stats.maxMP  += item.bonusMP  * mult;
        stats.atk    += item.bonusATK * mult;
        stats.def    += item.bonusDEF * mult;

        stats.currentHP = Mathf.Min(stats.currentHP, stats.maxHP);
        stats.currentMP = Mathf.Min(stats.currentMP, stats.maxMP);

        GameManager.Instance.currentHP = stats.currentHP;
        GameManager.Instance.currentMP = stats.currentMP;
    }

    public int GetTotalBonusATK()
    {
        int total = 0;
        foreach (var item in equipped.Values)
            if (item != null) total += item.bonusATK;
        return total;
    }
}
