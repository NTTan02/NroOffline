using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public string raceName;
    public int level, exp, gold, currentHP, currentMP;
    public int mapIndex;
    public SpawnSide spawnSide;
}

public static class SaveSystem
{
    const string KEY = "ChuBeRongSave";

    public static void Save()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.selectedRace == null) return;

        var data = new SaveData
        {
            playerName = gm.playerName,
            raceName   = gm.selectedRace.raceName,
            level      = gm.playerLevel,
            exp        = gm.playerEXP,
            gold       = gm.playerGold,
            currentHP  = gm.currentHP,
            currentMP  = gm.currentMP,
            mapIndex   = gm.currentMapIndex,
            spawnSide  = gm.targetSpawnSide,
        };

        PlayerPrefs.SetString(KEY, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
        Debug.Log("Game saved!");
    }

    // Trả về true nếu load thành công
    public static bool Load(CharacterData[] allRaces)
    {
        if (!HasSave()) return false;

        var data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(KEY));
        if (data == null) return false;

        var gm = GameManager.Instance;
        if (gm == null) return false;

        // Tìm lại race theo tên
        CharacterData foundRace = null;
        foreach (var race in allRaces)
        {
            if (race.raceName == data.raceName)
            {
                foundRace = race;
                break;
            }
        }

        if (foundRace == null)
        {
            Debug.LogError($"Không tìm thấy tộc: {data.raceName}");
            return false;
        }

        // Gán data vào GameManager
        gm.selectedRace      = foundRace;
        gm.playerName        = data.playerName;
        gm.playerLevel       = data.level;
        gm.playerEXP         = data.exp;
        gm.playerGold        = data.gold;
        gm.currentHP         = data.currentHP;
        gm.currentMP         = data.currentMP;
        gm.currentMapIndex   = data.mapIndex;
        gm.targetSpawnSide   = data.spawnSide;

        Debug.Log($"Loaded: {data.playerName} | Lv.{data.level} | Map {data.mapIndex}");
        return true;
    }

    public static bool HasSave() => PlayerPrefs.HasKey(KEY);
    public static void Delete()  => PlayerPrefs.DeleteKey(KEY);
}