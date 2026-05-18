using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    [Header("Tất cả quest trong game")]
    public QuestData[] allQuests;

    public List<QuestData> activeQuests = new List<QuestData>();
    public Dictionary<string, int> progress = new Dictionary<string, int>();
    public List<string> completedQuests = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public bool AcceptQuest(QuestData quest)
    {
        if (activeQuests.Contains(quest)) return false;
        if (completedQuests.Contains(quest.questName)) return false;

        activeQuests.Add(quest);
        progress[quest.questName] = 0;
        Debug.Log($"Nhận quest: {quest.questName}");
        return true;
    }

    public void OnEnemyKilled(string enemyName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questType == QuestType.KillEnemy
                && quest.targetName == enemyName)
            {
                progress[quest.questName]++;
                CheckComplete(quest);
                FindObjectOfType<QuestUI>()?.Refresh();
            }
        }
    }

    public void OnItemCollected(string itemName)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questType == QuestType.CollectItem
                && quest.targetName == itemName)
            {
                progress[quest.questName]++;
                CheckComplete(quest);
                FindObjectOfType<QuestUI>()?.Refresh();
            }
        }
    }

    void CheckComplete(QuestData quest)
    {
        if (progress[quest.questName] < quest.targetAmount) return;

        activeQuests.Remove(quest);
        completedQuests.Add(quest.questName);

        GameManager.Instance.AddEXP(quest.rewardEXP);
        GameManager.Instance.playerGold += quest.rewardGold;
        if (quest.rewardItem != null)
            Inventory.Instance.AddItem(quest.rewardItem);

        DamageNumber.Show(Vector3.zero, 0, Color.yellow,
            $"Hoàn thành: {quest.questName}!");
        FindObjectOfType<HUDManager>()?.UpdateHUD();

        Debug.Log($"Hoàn thành quest: {quest.questName}!");
    }

    public int GetProgress(QuestData quest)
    {
        return progress.ContainsKey(quest.questName)
            ? progress[quest.questName]
            : 0;
    }
}
