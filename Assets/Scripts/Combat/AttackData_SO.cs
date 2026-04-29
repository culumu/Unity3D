using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName ="Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;       //攻击范围

    public float skillRange;        //技能范围

    public float coolDown;          //攻击冷却

    public int minDamage;           //最小攻击数值

    public int maxDamage;           //最大攻击数值

    public float cirticalMultiplier;    //暴击之后的加成百分比

    public float criticalChance;        //暴击率


}
