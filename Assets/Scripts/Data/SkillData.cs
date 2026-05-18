using UnityEngine;

public enum SkillType { Projectile, AoE, Heal, Buff }
public enum SkillTarget { Enemy, Self, AllEnemies }

[CreateAssetMenu(fileName = "NewSkill", menuName = "Game/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Thông tin")]
    public string skillName;
    public Sprite skillIcon;
    [TextArea] public string description;

    [Header("Loại")]
    public SkillType skillType;
    public SkillTarget target;

    [Header("Chỉ số")]
    public int mpCost = 10;
    public float cooldown = 3f;
    public float damageMultiplier = 1.5f;
    public float healAmount = 0f;
    public float buffAmount = 0f;
    public float buffDuration = 0f;
    public float projectileSpeed = 10f;
    public float aoeRadius = 3f;
    public bool piercing = false;       // ← thêm: xuyên qua nhiều enemy

    [Header("Prefab")]
    public GameObject projectilePrefab;
    public GameObject effectPrefab;     // effect khi trúng / AoE
}