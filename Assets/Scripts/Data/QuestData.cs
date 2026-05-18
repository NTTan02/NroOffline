using UnityEngine;

public enum QuestType { KillEnemy, CollectItem, ReachMap }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Game/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Thông tin")]
    public string questName;
    [TextArea] public string description;

    [Header("Mục tiêu")]
    public QuestType questType;
    public string targetName;   // Tên enemy cần giết / item cần thu thập
    public int targetAmount;    // Số lượng cần

    [Header("Phần thưởng")]
    public int rewardEXP;
    public int rewardGold;
    public ItemData rewardItem;
}