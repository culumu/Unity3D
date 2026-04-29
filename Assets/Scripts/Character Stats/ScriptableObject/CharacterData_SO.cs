using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data",menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]

    public int maxHealth;           //最大血量

    public int currentHealth;       //当前血量

    public int baseDefence;         //基础防御

    public int currentDefence;      //当前防御

    [Header("kill")]
    public int killPoint;
    [Header("Level")]
    public int maxLevel;

    public int currentLevel;    //当前等级

    public int baseExp;

    public int currentExp;

    public float levelBuff;

    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;
        if (currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel+1,0,maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth += (int)(maxHealth * LevelMultiplier);

        currentLevel = maxHealth;

        Debug.Log("LEVEL UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}
