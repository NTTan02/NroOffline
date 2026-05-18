using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Thông tin")]
    public string enemyName;
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    [Header("Chỉ số")]
    public int hp = 60;
    public int atk = 12;
    public int def = 4;
    public float moveSpeed = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 1.2f;
    public float aggroRange = 5f;

    [Header("Phần thưởng")]
    public int expReward = 15;
    public int goldReward = 8;

    [Header("Skills")]
    public List<SkillData> skills;

    [Header("Boss")]
    public bool isBoss = false;
}