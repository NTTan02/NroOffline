using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour
{
    private PlayerStats stats;
    private Animator anim;
    private SpriteRenderer sr;
    private PlayerController controller;

    [Header("Vị trí bắn skill")]
    public Transform firePoint;

    private float[] cooldownTimers;
    private SkillData pendingSkill;
    private Vector2 fireDirection;
    private float pendingSkillTimeout = 0f;
    private PlayerCombat combat;

    public HUDManager hud;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        cooldownTimers = new float[5];
        combat = GetComponent<PlayerCombat>();

        if (hud == null)
            hud = FindObjectOfType<HUDManager>();
        InitSkillSlotUI();
    }

    void Update()
    {
        if (!stats.IsAlive) return;

        for (int i = 0; i < cooldownTimers.Length; i++)
            if (cooldownTimers[i] > 0)
                cooldownTimers[i] -= Time.deltaTime;

        if (pendingSkill != null)
        {
            pendingSkillTimeout -= Time.deltaTime;
            if (pendingSkillTimeout <= 0)
            {
                Debug.LogWarning("pendingSkill timeout! Force execute: " + pendingSkill.skillName);
                OnFireProjectile();
            }
        }

        HandleSkillInput();

        if (hud != null && stats.skills != null)
            hud.UpdateSkillCooldowns(cooldownTimers, stats.skills);
    }

    void InitSkillSlotUI()
    {
        if (stats?.skills == null) return;
        hud?.InitSkillSlots(stats.skills);
    }

    void HandleSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (combat != null)
            {
                combat.NormalAttack();
            }

            return;
        }

        KeyCode[] keys = {
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

        for (int i = 0; i < keys.Length && i < stats.skills.Count; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                TryUseSkill(i + 1);
            }
        }
    }

    public void TryUseSkill(int index)
    {
        if (stats.skills == null || index >= stats.skills.Count) return;

        var skill = stats.skills[index];
        if (skill == null) return;

        if (pendingSkill != null) return;

        if (cooldownTimers[index] > 0)
        {
            DamageNumber.Show(transform.position + Vector3.up,
                0, Color.gray, $"{Mathf.CeilToInt(cooldownTimers[index])}s");
            return;
        }

        if (!stats.UseMP(skill.mpCost))
        {
            DamageNumber.Show(transform.position + Vector3.up,
                0, Color.red, "Không đủ MP!");
            return;
        }

        cooldownTimers[index] = skill.cooldown;
        pendingSkill = skill;
        pendingSkillTimeout = 1.0f;

        if (combat == null || combat.currentTarget == null || combat.currentTarget.IsDead)
        {
            Debug.Log("Chưa chọn enemy để dùng skill!");
            return;
        }

        fireDirection = GetTargetDirection(combat.currentTarget.transform);
        FaceDirection(fireDirection);

        anim?.SetInteger("skillIndex", index + 1);
        anim?.SetTrigger("skill");
        hud?.UpdateHUD();
    }
    Vector2 GetTargetDirection(Transform target)
    {
        Vector2 dir = target.position - transform.position;
        return dir.normalized;
    }

    void FaceDirection(Vector2 dir)
    {
        if (dir.x > 0) sr.flipX = false;
        else if (dir.x < 0) sr.flipX = true;

        if (firePoint != null)
        {
            Vector3 fp = firePoint.localPosition;
            fp.x = Mathf.Abs(fp.x) * (dir.x >= 0 ? 1 : -1);
            firePoint.localPosition = fp;
        }
    }

    public void OnFireProjectile()
    {
        if (pendingSkill == null) return;

        var skill = pendingSkill;
        pendingSkill = null;
        pendingSkillTimeout = 0f;

        ExecuteSkill(skill);
    }

    void ExecuteSkill(SkillData skill)
    {
        switch (skill.skillType)
        {
            case SkillType.Projectile: FireProjectile(skill); break;
            case SkillType.AoE: DoAoE(skill); break;
            case SkillType.Heal: DoHeal(skill); break;
            case SkillType.Buff: DoBuff(skill); break;
        }
    }

    void FireProjectile(SkillData skill)
    {
        if (skill.projectilePrefab == null)
        {
            Debug.LogWarning($"{skill.skillName} thiếu projectilePrefab!");
            return;
        }

        Vector3 spawnPos = firePoint != null
            ? firePoint.position
            : transform.position + Vector3.up * 0.5f;

        var proj = Instantiate(skill.projectilePrefab, spawnPos, Quaternion.identity);
        var p = proj.GetComponent<Projectile>();
        if (p != null) p.Init(fireDirection, skill, stats.ATK);
    }

    void DoAoE(SkillData skill)
    {
        var hits = Physics2D.OverlapCircleAll(
            transform.position, skill.aoeRadius,
            LayerMask.GetMask("Enemy"));

        int dmg = Mathf.RoundToInt(stats.ATK * skill.damageMultiplier);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(dmg);
                DamageNumber.Show(hit.transform.position, dmg, Color.yellow);
            }
        }

        if (skill.effectPrefab != null)
            Instantiate(skill.effectPrefab, transform.position, Quaternion.identity);
    }

    void DoHeal(SkillData skill)
    {
        int heal = Mathf.RoundToInt(skill.healAmount);
        stats.HealHP(heal);
        DamageNumber.Show(transform.position + Vector3.up, heal, Color.green);
    }

    void DoBuff(SkillData skill)
    {
        stats.ApplyATKBuff(1f + skill.buffAmount, skill.buffDuration);
        DamageNumber.Show(transform.position + Vector3.up, 0, Color.cyan, "ATK UP!");
    }
}
