using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Info")]
    public string playerName;
    public CharacterData selectedRace;
    public int playerLevel = 1;
    public int playerEXP = 0;
    public int playerGold = 0;
    public int currentHP;
    public int currentMP;

    [Header("EXP")]
    public int baseEXP = 100;
    public float expMultiplier = 1.4f;

    [Header("Map")]
    public int currentMapIndex = 0;
    public SpawnSide targetSpawnSide = SpawnSide.Left;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void StartNewGame(CharacterData race, string name)
    {
        selectedRace = race;
        playerName = name;
        playerLevel = 1;
        playerEXP = 0;
        playerGold = 0;
        currentHP = GetMaxHP();
        currentMP = GetMaxMP();
        SceneManager.LoadScene("World_Map");
    }

    public int GetMaxHP() => selectedRace.baseHP + selectedRace.hpPerLevel * (playerLevel - 1);
    public int GetMaxMP() => selectedRace.baseMP + selectedRace.mpPerLevel * (playerLevel - 1);
    public int GetATK() => selectedRace.baseATK + selectedRace.atkPerLevel * (playerLevel - 1);
    public int GetDEF() => selectedRace.baseDEF + selectedRace.defPerLevel * (playerLevel - 1);

    public int GetEXPRequired() =>
        Mathf.RoundToInt(baseEXP * Mathf.Pow(expMultiplier, playerLevel - 1));

    public bool AddEXP(int amount)
    {
        playerEXP += amount;
        if (playerEXP >= GetEXPRequired())
        {
            playerEXP -= GetEXPRequired();
            playerLevel++;
            currentHP = GetMaxHP();
            currentMP = GetMaxMP();
            return true;
        }
        return false;
    }

    public void LoadScene(string name) => SceneManager.LoadScene(name);
    void OnApplicationQuit()
    {
        // Tự động lưu khi thoát
        if (selectedRace != null)
            SaveSystem.Save();
    }
}
