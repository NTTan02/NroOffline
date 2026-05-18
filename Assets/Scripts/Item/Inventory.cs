using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<ItemData> items = new List<ItemData>();
    public int maxSlots = 20;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Túi đồ đầy!");
            return false;
        }
        items.Add(item);
        return true;
    }

    public void UseItem(int index)
    {
        if (index >= items.Count) return;

        var item  = items[index];
        var stats = FindObjectOfType<PlayerStats>();
        if (stats == null) return;

        switch (item.itemType)
        {
            case ItemType.HPPotion:
                stats.HealHP(item.value);
                DamageNumber.Show(stats.transform.position, item.value, Color.green);
                break;

            case ItemType.MPPotion:
                stats.HealMP(item.value);
                DamageNumber.Show(stats.transform.position, item.value, Color.blue);
                break;

            case ItemType.AtkBuff:
                stats.ApplyATKBuff(1f + item.value / 100f, 30f);
                DamageNumber.Show(stats.transform.position, 0, Color.yellow, "ATK UP!");
                break;
        }

        items.RemoveAt(index);
        FindObjectOfType<HUDManager>()?.UpdateHUD();
    }

    public void RemoveItem(ItemData item) => items.Remove(item);
}