using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject questPanel;
    public Transform questContainer;
    public GameObject questSlotPrefab;

    [Header("Tabs")]
    public Button btnActive;
    public Button btnCompleted;

    [Header("Màu tab")]
    public Color activeTabColor = new Color(0.88f, 0.47f, 0.13f);
    public Color inactiveTabColor = new Color(0.2f, 0.2f, 0.4f);

    private bool isOpen = false;
    private bool showActive = true; 

    void Start()
    {
        questPanel.SetActive(false);

        btnActive?.onClick.AddListener(() =>
        {
            showActive = true;
            UpdateTabColors();
            Refresh();
        });

        btnCompleted?.onClick.AddListener(() =>
        {
            showActive = false;
            UpdateTabColors();
            Refresh();
        });

        UpdateTabColors();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TogglePanel();
    }

    void TogglePanel()
    {
        isOpen = !isOpen;
        questPanel.SetActive(isOpen);
        if (isOpen) Refresh();
        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void OpenPanel()
    {
        isOpen = true;
        questPanel.SetActive(true);
        Refresh();
        Time.timeScale = 0f;
    }

    void UpdateTabColors()
    {
        if (btnActive != null)
            btnActive.GetComponent<Image>().color =
                showActive ? activeTabColor : inactiveTabColor;

        if (btnCompleted != null)
            btnCompleted.GetComponent<Image>().color =
                showActive ? inactiveTabColor : activeTabColor;
    }

    public void Refresh()
    {
        if (QuestSystem.Instance == null)
        {
            Debug.LogWarning("QuestSystem chưa sẵn sàng!");
            return;
        }

        foreach (Transform child in questContainer)
            Destroy(child.gameObject);

        var qs = QuestSystem.Instance;

        if (showActive)
        {
            if (qs.activeQuests.Count == 0)
            {
                ShowEmptyText("Chưa có nhiệm vụ!\nTìm NPC để nhận nhiệm vụ.");
                return;
            }
            foreach (var quest in qs.activeQuests)
                SpawnQuestSlot(quest, false);
        }
        else
        {
            if (qs.completedQuests.Count == 0)
            {
                ShowEmptyText("Chưa hoàn thành nhiệm vụ nào!");
                return;
            }
            foreach (var questName in qs.completedQuests)
                SpawnCompletedSlot(questName);
        }
    }

    void SpawnQuestSlot(QuestData quest, bool completed)
    {
        var slot = Instantiate(questSlotPrefab, questContainer);
        var texts = slot.GetComponentsInChildren<TMP_Text>();
        var slider = slot.GetComponentInChildren<Slider>();

        int prog = QuestSystem.Instance.GetProgress(quest);

        foreach (var t in texts)
        {
            switch (t.gameObject.name)
            {
                case "QuestName":
                    t.text = quest.questName;
                    t.color = completed ? Color.gray : new Color(1f, 0.85f, 0f);
                    break;
                case "QuestDesc":
                    t.text = quest.description;
                    break;
                case "QuestProgress":
                    t.text = completed
                        ? "✓ Hoàn thành"
                        : $"{prog} / {quest.targetAmount}";
                    t.color = completed ? Color.green : Color.white;
                    break;
                case "QuestReward":
                    t.text = $"Thưởng: +{quest.rewardEXP} EXP   +{quest.rewardGold} G" +
                             (quest.rewardItem != null ? $"   +{quest.rewardItem.itemName}" : "");
                    break;
            }
        }

        if (slider != null)
        {
            slider.maxValue = quest.targetAmount;
            slider.value = completed ? quest.targetAmount : prog;
        }
    }

    void SpawnCompletedSlot(string questName)
    {
        var qs = QuestSystem.Instance;
        foreach (var q in qs.allQuests)
        {
            if (q.questName == questName)
            {
                SpawnQuestSlot(q, true);
                return;
            }
        }
    }

    void ShowEmptyText(string msg)
    {
        var go = new GameObject("EmptyText");
        go.transform.SetParent(questContainer, false);
        var tmp = go.AddComponent<TMPro.TMP_Text>();
        tmp.text = msg;
        tmp.fontSize = 14;
        tmp.color = new Color(0.6f, 0.6f, 0.6f);
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
    }

    public void ClosePanel()
    {
        isOpen = false;
        questPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
