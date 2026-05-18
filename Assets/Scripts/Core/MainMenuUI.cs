using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnNewGame;
    public Button btnContinue;
    public Button btnQuit;

    [Header("Loading")]
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TMP_Text loadingText;

    [Header("All Races — kéo tất cả ScriptableObject tộc vào")]
    public CharacterData[] allRaces;

    void Start()
    {
        btnNewGame.onClick.AddListener(OnNewGame);
        btnContinue.onClick.AddListener(OnContinue);
        btnQuit.onClick.AddListener(OnQuit);

        // Chỉ bật Continue nếu có save
        btnContinue.interactable = SaveSystem.HasSave();
        loadingPanel.SetActive(false);
    }

    void OnNewGame()
    {
        if (SaveSystem.HasSave()) SaveSystem.Delete();
        StartCoroutine(LoadScene("CharacterCreate"));
    }

    void OnContinue()
    {
        // Load data vào GameManager trước
        bool success = SaveSystem.Load(allRaces);

        if (!success)
        {
            Debug.LogError("Load thất bại!");
            // Fallback về CharacterCreate
            StartCoroutine(LoadScene("CharacterCreate"));
            return;
        }

        // Load thẳng vào World_Map
        StartCoroutine(LoadScene("World_Map"));
    }

    void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator LoadScene(string sceneName)
    {
        loadingPanel.SetActive(true);
        loadingBar.value = 0f;
        loadingText.text = "Đang tải...";

        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            loadingBar.value = op.progress;
            loadingText.text = $"Đang tải... {Mathf.RoundToInt(op.progress * 100)}%";
            yield return null;
        }

        loadingBar.value = 1f;
        loadingText.text = "Hoàn tất!";
        yield return new WaitForSeconds(0.4f);
        op.allowSceneActivation = true;
    }
}