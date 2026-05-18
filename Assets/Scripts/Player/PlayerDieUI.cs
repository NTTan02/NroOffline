using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerDieUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject diePanel;
    public TMP_Text titleText;
    public TMP_Text goldCostText;
    public Button btnRevive;

    [Header("Cài đặt")]
    public int reviveCostPercent = 10; // Trừ 10% gold hiện có

    void Start()
    {
        diePanel.SetActive(false);
        btnRevive.onClick.AddListener(OnRevive);
    }

    public void ShowDiePanel()
    {
        diePanel.SetActive(true);
        Time.timeScale = 0f;

        int cost = GetReviveCost();
        goldCostText.text = $"Hồi sinh tốn {cost} Vàng\n" +
                            $"(Vàng hiện có: {GameManager.Instance.playerGold} G)";

        btnRevive.interactable = GameManager.Instance.playerGold >= cost;
        if (!btnRevive.interactable)
            goldCostText.text += "\n<color=#FF4444>Không đủ vàng!</color>";
    }

    int GetReviveCost()
    {
        int gold = GameManager.Instance.playerGold;
        return Mathf.Max(100, gold * reviveCostPercent / 100);
    }

    void OnRevive()
    {
        int cost = GetReviveCost();
        GameManager.Instance.playerGold -= cost;

        var stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            stats.currentHP = Mathf.Max(1,
                Mathf.RoundToInt(stats.maxHP * 1f));
            stats.currentMP = Mathf.Max(1,
                Mathf.RoundToInt(stats.maxMP * 1f));

            GameManager.Instance.currentHP = stats.currentHP;
            GameManager.Instance.currentMP = stats.currentMP;

            Animator anim = stats.GetComponent<Animator>();

            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);

                anim.ResetTrigger("die");
                anim.CrossFade("idel", 0.1f);
            }
            stats.GetComponent<PlayerController>()?.SetCanMove(true);
        }

        diePanel.SetActive(false);
        Time.timeScale = 1f;

        FindObjectOfType<HUDManager>()?.UpdateHUD();

        DamageNumber.Show(
            stats?.transform.position ?? Vector3.zero,
            0, Color.green, "Hồi sinh!");
    }
}
