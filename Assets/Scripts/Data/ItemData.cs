using UnityEngine;

public enum EquipmentType
{
    Ao,      // Áo
    Quan,    // Quần
    Gang,    // Găng
    Giay,    // Giày
    Rada,    // Rada
    Giap     // Giáp luyện tập
}
public enum ItemType { HPPotion, MPPotion, AtkBuff, DefBuff, Equipment }

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Thông tin")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int price;
    [TextArea] public string description;

    [Header("Potion")]
    public int value;

    [Header("Equipment")]
    public EquipmentType equipmentType;  // Loại trang bị
    public Sprite characterOverlay;      // Sprite hiện lên người khi mặc (optional)

    [Header("Equipment Stats")]
    public int bonusHP;
    public int bonusMP;
    public int bonusATK;
    public int bonusDEF;
    public int bonusSPD;
}