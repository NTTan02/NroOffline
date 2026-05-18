using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Tất cả map")]
    public MapData[] maps;

    [Header("References")]
    public GameObject playerPrefab;
    public CameraFollow cameraFollow;

    private GameObject currentMapInstance;
    private GameObject currentPlayer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadMap(GameManager.Instance.currentMapIndex);
    }

    public void LoadMap(int index)
    {
        StartCoroutine(DoLoadMap(index));
    }

    IEnumerator DoLoadMap(int index)
    {
        if (index >= maps.Length || index < 0)
        {
            Debug.LogWarning("Map index không hợp lệ: " + index);
            yield break;
        }

        if (currentMapInstance != null) Destroy(currentMapInstance);
        if (currentPlayer != null)      Destroy(currentPlayer);

        yield return null;

        currentMapInstance = Instantiate(maps[index].mapPrefab);

        SpawnSide side = GameManager.Instance.targetSpawnSide;
        string spawnName = side == SpawnSide.Left
            ? "SpawnPoint_Left"
            : "SpawnPoint_Right";

        Transform spawnPoint = currentMapInstance.transform.Find(spawnName);

        if (spawnPoint == null)
        {
            Debug.LogWarning($"Không tìm thấy {spawnName}, dùng vị trí mặc định!");
            spawnPoint = currentMapInstance.transform.Find("SpawnPoint_Right");
        }

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        cameraFollow.target = currentPlayer.transform;


        FindObjectOfType<HUDManager>()?.UpdateHUD();

        Debug.Log($"Loaded: {maps[index].mapName} | Spawn: {spawnName}");
    }
}
