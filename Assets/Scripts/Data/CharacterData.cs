using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRace", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Thông tin tộc")]
    public string raceName;
    [Header("Animation")]
    public RuntimeAnimatorController animatorController;
    public Sprite characterSprite;
    [Header("Chỉ số gốc")]
    public int baseHP = 100;
    public int baseMP = 80;
    public int baseATK = 18;
    public int baseDEF = 12;
    public float baseSPD = 5f;
    public float baseATKSpeed = 1f; // Số lần attack mỗi giây

    [Header("Tăng mỗi level")]
    public int hpPerLevel = 15;
    public int mpPerLevel = 10;
    public int atkPerLevel = 2;
    public int defPerLevel = 1;

    [Header("Skills mặc định")]
    public List<SkillData> defaultSkills;

    [Header("Mô tả")]
    [TextArea] public string description;
    public string passiveDescription;
}