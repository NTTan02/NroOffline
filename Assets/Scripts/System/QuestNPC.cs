using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    [Header("Quest NPC cung cấp")]
    public QuestData[] availableQuests;

    [Header("UI")]
    public GameObject interactCanvas;

    private bool playerNearby = false;

    void Start()
    {
        interactCanvas?.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
            OpenQuestPanel();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactCanvas?.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactCanvas?.SetActive(false);
        }
    }

    void OpenQuestPanel()
    {
        if (QuestSystem.Instance == null)
        {
            Debug.LogError("QuestSystem.Instance null!");
            return;
        }

        foreach (var quest in availableQuests)
        {
            bool accepted = QuestSystem.Instance.AcceptQuest(quest);
            if (accepted)
                DamageNumber.Show(transform.position + Vector3.up * 2,
                    0, Color.yellow, $"Nhận: {quest.questName}!");
        }

        FindObjectOfType<QuestUI>()?.OpenPanel();
    }
}