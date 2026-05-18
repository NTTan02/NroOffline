using UnityEngine;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public int maxHP, currentHP;
    [HideInInspector] public int maxMP, currentMP;
    [HideInInspector] public int atk, def;
    [HideInInspector] public float moveSpeed, atkSpeed;
    [HideInInspector] public List<SkillData> skills = new List<SkillData>();

    private float atkBuff = 1f;
    private float atkBuffTimer = 0f;
    public int ATK => Mathf.RoundToInt(atk * atkBuff);
    public bool IsAlive => currentHP > 0;

    void Start()
    {
        InitFromGameManager();
    }

    public void InitFromGameManager()
    {
        var gm = GameManager.Instance;
        var race = gm.selectedRace;

        if (race == null)
        {
            Debug.LogError("Chưa chọn tộc!");
            return;
        }

        // Gán stats
        maxHP = gm.GetMaxHP();
        currentHP = gm.currentHP;
        maxMP = gm.GetMaxMP();
        currentMP = gm.currentMP;
        atk = gm.GetATK();
        def = gm.GetDEF();
        moveSpeed = race.baseSPD;
        atkSpeed = race.baseATKSpeed;
        skills = new List<SkillData>(race.defaultSkills);

        // Gán Sprite theo tộc
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && race.characterSprite != null)
            sr.sprite = race.characterSprite;

        // Gán Animator Controller theo tộc
        var anim = GetComponent<Animator>();
        if (anim != null && race.animatorController != null)
            anim.runtimeAnimatorController = race.animatorController;
    }

    void Update()
    {
        if (atkBuffTimer > 0)
        {
            atkBuffTimer -= Time.deltaTime;
            if (atkBuffTimer <= 0) atkBuff = 1f;
        }
    }

    public void ApplyATKBuff(float multiplier, float duration)
    {
        atkBuff = multiplier;
        atkBuffTimer = duration;
    }

    public int TakeDamage(int rawDmg)
    {
        int dmg = Mathf.Max(1, rawDmg - def);
        currentHP = Mathf.Max(0, currentHP - dmg);
        GameManager.Instance.currentHP = currentHP;

        GetComponent<Animator>()?.SetTrigger("hit");
        // AudioManager.Instance?.PlayHurt();

        // Kiểm tra chết
        if (currentHP <= 0)
            OnDie();

        return dmg;
    }

    void OnDie()
    {
        GetComponent<Animator>()?.SetTrigger("die");
        GetComponent<PlayerController>()?.SetCanMove(false);

        // Hiện Die Panel sau 1 giây
        StartCoroutine(ShowDiePanelDelay());
    }

    System.Collections.IEnumerator ShowDiePanelDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        FindObjectOfType<PlayerDieUI>()?.ShowDiePanel();
    }

    public void HealHP(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        GameManager.Instance.currentHP = currentHP;
    }

    public bool UseMP(int amount)
    {
        if (currentMP < amount) return false;
        currentMP -= amount;
        GameManager.Instance.currentMP = currentMP;
        return true;
    }

    public void HealMP(int amount)
    {
        currentMP = Mathf.Min(maxMP, currentMP + amount);
        GameManager.Instance.currentMP = currentMP;
    }
}