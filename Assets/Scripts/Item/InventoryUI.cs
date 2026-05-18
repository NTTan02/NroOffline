using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Panel chính")]
    public GameObject inventoryPanel;
    public Transform itemGrid;
    public GameObject inventorySlotPrefab;

    [Header("Item Info Popup")]
    public GameObject itemInfoPopup;
    public Image popupIcon;
    public TMP_Text popupName;
    public TMP_Text popupDesc;
    public TMP_Text popupStats;
    public Button btnUse;
    public Button btnDrop;

    private bool isOpen = false;
    private int selectedIndex = -1;

    [Header("Nút trang bị")]
    public Button btnEquip;

    void Start()
    {
        itemInfoPopup?.SetActive(false);

        btnUse?.onClick.AddListener(UseSelectedItem);
        btnDrop?.onClick.AddListener(DropSelectedItem);

        btnEquip?.onClick.AddListener(EquipSelectedItem);
        btnEquip?.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            ToggleInventory();

        // Bấm ESC đóng popup
        if (Input.GetKeyDown(KeyCode.Escape))
            HideItemInfo();
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
            RefreshInventory();
        else
            HideItemInfo();

        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void RefreshInventory()
    {
        // Xóa slot cũ
        foreach (Transform child in itemGrid)
            Destroy(child.gameObject);

        var items = Inventory.Instance.items;
        for (int i = 0; i < items.Count; i++)
        {
            var go = Instantiate(inventorySlotPrefab, itemGrid);
            var slot = go.GetComponent<InventorySlot>();
            slot?.Setup(items[i], i, this);
        }
    }

    public void ShowItemInfo(ItemData item, int index)
    {
        if (item == null) return;

        selectedIndex = index;
        itemInfoPopup?.SetActive(true);

        // Gán icon
        if (popupIcon != null)
        {
            popupIcon.sprite = item.icon;
            popupIcon.color = item.icon != null ? Color.white : Color.clear;
        }

        // Gán tên
        if (popupName != null)
            popupName.text = item.itemName;

        // Gán mô tả
        if (popupDesc != null)
            popupDesc.text = item.description;

        // Gán stats
        if (popupStats != null)
        {
            switch (item.itemType)
            {
                case ItemType.HPPotion:
                    popupStats.text = $"<color=#00FF00>HP +{item.value}</color>";
                    break;
                case ItemType.MPPotion:
                    popupStats.text = $"<color=#00AAFF>MP +{item.value}</color>";
                    break;
                case ItemType.AtkBuff:
                    popupStats.text = $"<color=#FFD700>ATK +{item.value}%</color>\n" +
                                      $"<color=#AAAAAA>Thời gian: 30 giây</color>";
                    break;
                case ItemType.Equipment:
                    string s = "";
                    if (item.bonusHP != 0) s += $"<color=#00FF00>HP  +{item.bonusHP}</color>\n";
                    if (item.bonusMP != 0) s += $"<color=#00AAFF>MP  +{item.bonusMP}</color>\n";
                    if (item.bonusATK != 0) s += $"<color=#FFD700>ATK +{item.bonusATK}</color>\n";
                    if (item.bonusDEF != 0) s += $"<color=#AAFFAA>DEF +{item.bonusDEF}</color>\n";
                    if (item.bonusSPD != 0) s += $"<color=#FF88FF>SPD +{item.bonusSPD}</color>\n";
                    popupStats.text = s;
                    break;
            }
        }

        // ===== Xử lý nút theo loại item =====
        bool isEquipment = item.itemType == ItemType.Equipment;

        // Nút Sử dụng — chỉ hiện với Potion/Buff
        if (btnUse != null)
        {
            btnUse.gameObject.SetActive(!isEquipment);
        }

        // Nút Trang bị — chỉ hiện với Equipment
        if (btnEquip != null)
        {
            btnEquip.gameObject.SetActive(isEquipment);

            // Kiểm tra slot đã có đồ chưa
            if (isEquipment && EquipmentSystem.Instance != null)
            {
                var currentEquipped = EquipmentSystem.Instance.equipped[item.equipmentType];
                btnEquip.GetComponentInChildren<TMPro.TMP_Text>().text =
                    currentEquipped != null ? "Thay trang bị" : "Trang bị";
            }
        }
    }
    void EquipSelectedItem()
    {
        if (selectedIndex < 0) return;
        var item = Inventory.Instance.items[selectedIndex];
        EquipmentSystem.Instance?.Equip(item);
        HideItemInfo();
    }
    public void HideItemInfo()
    {
        itemInfoPopup?.SetActive(false);
        selectedIndex = -1;
    }

    void UseSelectedItem()
    {
        if (selectedIndex < 0) return;
        Inventory.Instance.UseItem(selectedIndex);
        HideItemInfo();
        RefreshInventory();
        FindObjectOfType<HUDManager>()?.UpdateHUD();
    }

    void DropSelectedItem()
    {
        if (selectedIndex < 0) return;
        Inventory.Instance.items.RemoveAt(selectedIndex);
        HideItemInfo();
        RefreshInventory();
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        HideItemInfo();
        Time.timeScale = 1f;
    }
}