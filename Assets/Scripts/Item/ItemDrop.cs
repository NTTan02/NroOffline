using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public ItemData itemData;
    private bool collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // Thêm vào inventory
        bool added = Inventory.Instance.AddItem(itemData);

        if (added)
        {
            DamageNumber.Show(transform.position, 0,
                Color.cyan, $"+{itemData.itemName}");
            FindObjectOfType<InventoryUI>()?.RefreshInventory();
        }
        else
        {
            DamageNumber.Show(transform.position, 0,
                Color.red, "Túi đồ đầy!");
        }
        QuestSystem.Instance?.OnItemCollected(itemData.itemName);
        Destroy(gameObject);
    }
}