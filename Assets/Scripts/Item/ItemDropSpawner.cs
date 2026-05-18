using UnityEngine;

public class ItemDropSpawner : MonoBehaviour
{
    [Header("Drop config")]
    public ItemDrop[] possibleDrops;  
    public float[] dropRates;         
    public GameObject dropPrefab;    
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

        Vector3 spawnPos = pos + Vector3.up * 0.5f;
        var go = Instantiate(dropPrefab, spawnPos, Quaternion.identity);
        go.GetComponent<ItemDrop>().itemData = itemDrop.itemData;

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(
                Random.Range(-2f, 2f),
                Random.Range(3f, 6f));
        }
    }
}
