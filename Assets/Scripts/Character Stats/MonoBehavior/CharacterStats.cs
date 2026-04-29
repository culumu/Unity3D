using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdataHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;

    public AttackData_SO attackData;


    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
    }

    //#region可以折叠起来，方便读写
    #region Read from Data_SO
    //代表既可以被写入又可以被读取
    public int MaxHealth
    {
        /*get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }*/
        get => (characterData != null) ? characterData.maxHealth : 0;
        //value代表外部对这个属性赋的值
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get => (characterData != null) ? characterData.currentHealth : 0;
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get => (characterData != null) ? characterData.baseDefence : 0;
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get => (characterData != null) ? characterData.currentDefence : 0;
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence,0);  //要是攻击力少于防御力 酒曲伤害为1
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if(attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:Update UI
        UpdataHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);


        //TODO:经验Update
        if(CurrentHealth<=0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence,0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdataHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if(isCritical)
        {
            coreDamage *= attackData.cirticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
