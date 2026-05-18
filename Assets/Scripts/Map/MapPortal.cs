using UnityEngine;
using System.Collections;

public class MapPortal : MonoBehaviour
{
    [Header("Map đích")]
    public int targetMapIndex;

    [Header("Spawn ở cổng nào của map đích")]
    public SpawnSide spawnSide;

    private bool isTransitioning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
            StartCoroutine(GoToMap(other.gameObject));
    }

    IEnumerator GoToMap(GameObject player)
    {
        isTransitioning = true;

        // Lưu HP/MP
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            GameManager.Instance.currentHP = stats.currentHP;
            GameManager.Instance.currentMP = stats.currentMP;
        }

        // Lưu map index + spawn side
        GameManager.Instance.currentMapIndex  = targetMapIndex;
        GameManager.Instance.targetSpawnSide  = spawnSide;

        // AudioManager.Instance?.PlayPortal();
        SaveSystem.Save();

        player.GetComponent<PlayerController>()?.SetCanMove(false);
        yield return new WaitForSeconds(0.3f);

        MapManager.Instance.LoadMap(targetMapIndex);
    }
}

public enum SpawnSide { Left, Right }