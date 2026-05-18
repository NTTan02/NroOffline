using UnityEngine;

public enum EquipmentType
{
    Ao,     
    Quan,   
    Gang,    
    Giay,    
    Rada,   
    Giap    
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
    public EquipmentType equipmentType;  
    public Sprite characterOverlay;      
    [Header("Equipment Stats")]
    public int bonusHP;
    public int bonusMP;
    public int bonusATK;
    public int bonusDEF;
    public int bonusSPD;
}
