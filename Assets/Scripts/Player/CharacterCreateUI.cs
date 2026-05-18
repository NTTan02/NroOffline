using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCreateUI : MonoBehaviour
{
    [Header("Data")]
    public CharacterData[] races;

    [Header("UI")]
    public TMP_InputField nameInput;
    public Button[] raceButtons;
    public TMP_Text raceNameText;
    public TMP_Text statsText;
    public TMP_Text descText;
    public Image characterPreview;
    public Button btnCreate;
    public Button btnBack;

    private CharacterData selectedRace;
    private int selectedIndex = -1;

    void Start()
    {
        btnCreate.onClick.AddListener(OnCreate);
        btnBack.onClick.AddListener(() => GameManager.Instance.LoadScene("MainMenu"));
        btnCreate.interactable = false;

        for (int i = 0; i < raceButtons.Length; i++)
        {
            int idx = i;
            raceButtons[i].onClick.AddListener(() => SelectRace(idx));
        }

        for (int i = 0; i < raceButtons.Length && i < races.Length; i++)
            raceButtons[i].GetComponentInChildren<TMP_Text>().text = races[i].raceName;
    }

    void SelectRace(int idx)
    {
        selectedIndex = idx;
        selectedRace = races[idx];
        
        foreach (var btn in raceButtons)
            btn.GetComponent<Image>().color = new Color(0.9f, 0.6f, 0.2f);

        // Highlight nút được chọn
        raceButtons[idx].GetComponent<Image>().color = new Color(0.3f, 0.8f, 0.3f);

        // Cập nhật info
        raceNameText.text = selectedRace.raceName;
        statsText.text =
            $"HP: {selectedRace.baseHP}   MP: {selectedRace.baseMP}\n" +
            $"ATK: {selectedRace.baseATK}  DEF: {selectedRace.baseDEF}\n" +
            $"SPD: {selectedRace.baseSPD}";
        descText.text = selectedRace.description;

        if (selectedRace.characterSprite != null)
            characterPreview.sprite = selectedRace.characterSprite;

        btnCreate.interactable = nameInput.text.Length >= 2;
    }

    void OnCreate()
    {
        string pName = nameInput.text.Trim();
        if (pName.Length < 2 || selectedRace == null) return;
        GameManager.Instance.StartNewGame(selectedRace, pName);
    }

    void Update()
    {
        // Bật nút Tạo mới khi đủ điều kiện
        btnCreate.interactable = nameInput.text.Trim().Length >= 2 && selectedRace != null;
    }
}