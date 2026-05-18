using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{

    [Header("Enemy info")]
    public GameObject enemyInfoPanel;
    public TMP_Text enemyNameText;
    public Image enemyAvatar;
    public Slider enemyHPBar;
    public TMP_Text enemyHPText;

    [Header("Player Info")]
    public TMP_Text playerNameText;
    public Image playerAvatar;

    [Header("HP Bar")]
    public Slider hpBar;
    public TMP_Text hpText;

    [Header("MP Bar")]
    public Slider mpBar;
    public TMP_Text mpText;

    [Header("EXP Bar")]
    public Slider expBar;
    public TMP_Text levelText;

    [Header("Gold")]
    public TMP_Text goldText;

    [Header("Skill Slots (5 slot)")]
    public Image[] skillIcons;         // Icon của từng skill
    public TMP_Text[] skillKeyTexts;   // Hiện phím 1-5
    public Image[] cooldownOverlays;   // Overlay tối khi hồi chiêu
    public TMP_Text[] cooldownTexts;   // Số giây còn lại

    [Header("Level Up")]
    public GameObject levelUpPanel;    // Panel "LEVEL UP!"
    public TMP_Text levelUpText;

    [Header("Gold Popup")]
    public TMP_Text goldPopupText;     // Hiện "+8 Gold" khi nhận

    private PlayerStats playerStats;

    [Header("Interact Hint")]
    public GameObject interactHint;

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();

        levelUpPanel?.SetActive(false);
        goldPopupText?.gameObject.SetActive(false);

        InitSkillSlots(); // thêm dòng này

        UpdateHUD();
        UpdatePlayerInfo();
        HideEnemyInfo();
    }

    // ===== KHỞI TẠO SKILL SLOT =====

    void InitSkillSlots()
    {
        if (playerStats == null) return;

        string[] keyLabels = { "1", "2", "3", "4", "5" };

        for (int i = 0; i < skillIcons.Length; i++)
        {
            // Hiện icon skill nếu có
            if (i < playerStats.skills.Count && playerStats.skills[i] != null)
            {
                var skill = playerStats.skills[i];
                skillIcons[i].sprite = skill.skillIcon;
                skillIcons[i].color = Color.white;
            }
            else
            {
                // Slot trống
                skillIcons[i].color = new Color(1, 1, 1, 0.2f);
            }

            // Hiện phím tắt
            if (skillKeyTexts != null && i < skillKeyTexts.Length)
                skillKeyTexts[i].text = keyLabels[i];

            // Reset cooldown overlay
            if (cooldownOverlays != null && i < cooldownOverlays.Length)
                cooldownOverlays[i].fillAmount = 0f;

            if (cooldownTexts != null && i < cooldownTexts.Length)
                cooldownTexts[i].text = "";
        }
    }
    // Gọi từ SkillSystem khi init
    public void InitSkillSlots(List<SkillData> skills)
    {
        if (skills == null) return;

        string[] keyLabels = { "1", "2", "3", "4", "5" };

        for (int i = 0; i < skillIcons.Length; i++)
        {
            if (i < skills.Count && skills[i] != null)
            {
                skillIcons[i].sprite = skills[i].skillIcon;
                skillIcons[i].color = skills[i].skillIcon != null
                    ? Color.white
                    : new Color(1, 1, 1, 0.2f);
            }
            else
            {
                skillIcons[i].color = new Color(1, 1, 1, 0.2f);
            }

            if (skillKeyTexts != null && i < skillKeyTexts.Length)
                skillKeyTexts[i].text = keyLabels[i];

            if (cooldownOverlays != null && i < cooldownOverlays.Length)
                cooldownOverlays[i].fillAmount = 0f;

            if (cooldownTexts != null && i < cooldownTexts.Length)
                cooldownTexts[i].text = "";
        }
    }

    // ===== CẬP NHẬT HUD CHÍNH =====

    public void UpdateHUD()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        int maxHP = gm.GetMaxHP();
        int maxMP = gm.GetMaxMP();
        int maxEXP = gm.GetEXPRequired();

        // HP
        hpBar.maxValue = maxHP;
        hpBar.value = gm.currentHP;
        hpText.text = $"{gm.currentHP} / {maxHP}";

        // MP
        mpBar.maxValue = maxMP;
        mpBar.value = gm.currentMP;
        mpText.text = $"{gm.currentMP} / {maxMP}";

        // EXP
        expBar.maxValue = maxEXP;
        expBar.value = gm.playerEXP;

        // Level & Gold
        levelText.text = $"Lv.{gm.playerLevel}";
        goldText.text = $"{gm.playerGold} G";
    }

    // ===== CẬP NHẬT THÔNG TIN NHÂN VẬT =====

    void UpdatePlayerInfo()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        playerNameText.text = gm.playerName;

        if (gm.selectedRace?.characterSprite != null)
            playerAvatar.sprite = gm.selectedRace.characterSprite;
    }

    // ===== CẬP NHẬT COOLDOWN SKILL =====

    public void UpdateSkillCooldowns(float[] timers, List<SkillData> skills)
    {
        for (int i = 0; i < cooldownOverlays.Length && i < timers.Length; i++)
        {
            float maxCD = i < skills.Count ? skills[i].cooldown : 1f;
            float remaining = timers[i];

            // Overlay tối theo tỉ lệ cooldown còn lại
            if (cooldownOverlays[i] != null)
                cooldownOverlays[i].fillAmount = remaining > 0
                    ? remaining / maxCD
                    : 0f;

            // Text đếm ngược
            if (cooldownTexts[i] != null)
                cooldownTexts[i].text = remaining > 0
                    ? Mathf.CeilToInt(remaining).ToString()
                    : "";
        }
    }

    // ===== LEVEL UP =====

    public void ShowLevelUp()
    {
        UpdateHUD();
        StartCoroutine(LevelUpEffect());
    }

    IEnumerator LevelUpEffect()
    {
        levelUpPanel.SetActive(true);
        levelUpText.text = $"LEVEL UP!  Lv.{GameManager.Instance.playerLevel}";
        yield return new WaitForSeconds(2.5f);
        levelUpPanel.SetActive(false);
    }

    // ===== GOLD POPUP =====

    public void ShowGoldPopup(int amount)
    {
        StartCoroutine(GoldPopupEffect(amount));
    }

    IEnumerator GoldPopupEffect(int amount)
    {
        goldPopupText.gameObject.SetActive(true);
        goldPopupText.text = $"+{amount} G";
        yield return new WaitForSeconds(1.5f);
        goldPopupText.gameObject.SetActive(false);
    }

    public void ShowEnemyInfo(EnemyBase enemy)
    {
        if (enemy == null || enemy.data == null)
        {
            HideEnemyInfo();
            return;
        }

        if (enemyInfoPanel != null)
            enemyInfoPanel.SetActive(true);

        if (enemyNameText != null)
            enemyNameText.text = enemy.data.enemyName;

        if (enemyHPBar != null)
        {
            enemyHPBar.maxValue = enemy.data.hp;
            enemyHPBar.value = enemy.currentHP;
        }

        if (enemyHPText != null)
            enemyHPText.text = enemy.currentHP + " / " + enemy.data.hp;

        if (enemyAvatar != null && enemy.data.sprite != null)
        {
            enemyAvatar.sprite = enemy.data.sprite;
            enemyAvatar.color = Color.white;
        }
    }

    public void UpdateEnemyInfo(EnemyBase enemy)
    {
        if (enemy == null || enemy.data == null)
        {
            HideEnemyInfo();
            return;
        }

        if (enemyHPBar != null)
            enemyHPBar.value = enemy.currentHP;

        if (enemyHPText != null)
            enemyHPText.text = enemy.currentHP + " / " + enemy.data.hp;
    }

    public void HideEnemyInfo()
    {
        if (enemyInfoPanel != null)
            enemyInfoPanel.SetActive(false);
    }

    public void ShowInteractHint(bool show)
    {
        interactHint?.SetActive(show);
    }
}