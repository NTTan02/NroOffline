using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    [Header("Tất cả quest trong game")]
    public QuestData[] allQuests;

    // Quest đang làm
    public List<QuestData> activeQuests = new List<QuestData>();
    // Tiến độ quest (questName → số lượng hiện tại)
    public Dictionary<string, int> progress = new Dictionary<string, int>();
    // Quest đã hoàn thành
    public List<string> completedQuests = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ← thêm dòng này
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Nhận quest
    public bool AcceptQuest(QuestData quest)
    {
        if (activeQuests.Contains(quest)) return false;
        if (completedQuests.Contains(quest.questName)) return false;

        activeQuests.Add(quest);
        progress[quest.questName] = 0;
        Debug.Log($"Nhận quest: {quest.questName}");
        return true;
    }

    // Cập nhật tiến độ khi giết enemy
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

    // Cập nhật tiến độ khi nhặt item
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

    // Kiểm tra hoàn thành
    void CheckComplete(QuestData quest)
    {
        if (progress[quest.questName] < quest.targetAmount) return;

        // Hoàn thành quest
        activeQuests.Remove(quest);
        completedQuests.Add(quest.questName);

        // Nhận thưởng
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