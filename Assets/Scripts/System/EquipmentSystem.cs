using UnityEngine;
using System.Collections.Generic;

public class EquipmentSystem : MonoBehaviour
{
    public static EquipmentSystem Instance;

    // Slot trang bị hiện tại
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

        // Khởi tạo slot rỗng
        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
            equipped[type] = null;
    }

    // Mặc trang bị
    public void Equip(ItemData item)
    {
        if (item == null || item.itemType != ItemType.Equipment) return;

        // Nếu slot đã có đồ → tháo ra trước
        if (equipped[item.equipmentType] != null)
            Unequip(item.equipmentType);

        equipped[item.equipmentType] = item;
        ApplyStats(item, true);

        // Xóa khỏi inventory
        Inventory.Instance.items.Remove(item);

        FindObjectOfType<HUDManager>()?.UpdateHUD();
        FindObjectOfType<EquipmentUI>()?.Refresh();
        FindObjectOfType<InventoryUI>()?.RefreshInventory();

        Debug.Log($"Mặc: {item.itemName}");
    }

    // Tháo trang bị
    public void Unequip(EquipmentType type)
    {
        var item = equipped[type];
        if (item == null) return;

        equipped[type] = null;
        ApplyStats(item, false);

        // Trả về inventory
        Inventory.Instance.AddItem(item);

        FindObjectOfType<HUDManager>()?.UpdateHUD();
        FindObjectOfType<EquipmentUI>()?.Refresh();
        FindObjectOfType<InventoryUI>()?.RefreshInventory();

        Debug.Log($"Tháo: {item.itemName}");
    }

    // Cộng/trừ stats
    void ApplyStats(ItemData item, bool add)
    {
        int mult = add ? 1 : -1;
        stats.maxHP  += item.bonusHP  * mult;
        stats.maxMP  += item.bonusMP  * mult;
        stats.atk    += item.bonusATK * mult;
        stats.def    += item.bonusDEF * mult;

        // Đảm bảo HP không vượt max
        stats.currentHP = Mathf.Min(stats.currentHP, stats.maxHP);
        stats.currentMP = Mathf.Min(stats.currentMP, stats.maxMP);

        // Cập nhật GameManager
        GameManager.Instance.currentHP = stats.currentHP;
        GameManager.Instance.currentMP = stats.currentMP;
    }

    // Lấy tổng bonus stat để hiện trong UI
    public int GetTotalBonusATK()
    {
        int total = 0;
        foreach (var item in equipped.Values)
            if (item != null) total += item.bonusATK;
        return total;
    }
}