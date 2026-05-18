using UnityEngine;

[CreateAssetMenu(fileName = "NewMapData", menuName = "Game/Map Data")]
public class MapData : ScriptableObject
{
    public string mapName;
    public GameObject mapPrefab;
    public int recommendedLevel; // Level đề xuất để vào map này
    public AudioClip bgMusic;
}