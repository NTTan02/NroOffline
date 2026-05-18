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
    public string targetName;  
    public int targetAmount; 

    [Header("Phần thưởng")]
    public int rewardEXP;
    public int rewardGold;
    public ItemData rewardItem;
}
