using UnityEngine;

public class ItemDropSpawner : MonoBehaviour
{
    [Header("Drop config")]
    public ItemDrop[] possibleDrops;  // Các item có thể rơi
    public float[] dropRates;         // Tỉ lệ rơi tương ứng (0-1)
    public GameObject dropPrefab;     // Prefab item rơi xuống đất

    public void TryDrop(Vector3 position)
    {
        for (int i = 0; i < possibleDrops.Length; i++)
        {
            float roll = Random.Range(0f, 1f);
            if (roll <= dropRates[i])
            {
                SpawnDrop(possibleDrops[i], position);
            }
        }
    }

    void SpawnDrop(ItemDrop itemDrop, Vector3 pos)
    {
        if (dropPrefab == null) return;

        // Spawn item rơi lên rồi rơi xuống
        Vector3 spawnPos = pos + Vector3.up * 0.5f;
        var go = Instantiate(dropPrefab, spawnPos, Quaternion.identity);
        go.GetComponent<ItemDrop>().itemData = itemDrop.itemData;

        // Bật Rigidbody để rơi tự nhiên
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(
                Random.Range(-2f, 2f),
                Random.Range(3f, 6f));
        }
    }
}