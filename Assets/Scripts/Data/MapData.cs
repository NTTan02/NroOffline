using UnityEngine;

[CreateAssetMenu(fileName = "NewMapData", menuName = "Game/Map Data")]
public class MapData : ScriptableObject
{
    public string mapName;
    public GameObject mapPrefab;
    public int recommendedLevel; 
    public AudioClip bgMusic;
}
