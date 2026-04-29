using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("skill")]
    public float kickForce = 25;

    public GameObject rockPrefab;   //石头的预制件
    public Transform handPos;       //举手的位置


    public void kickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //击退实现
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized; //攻击目标的位置向量-自己的位置向量
            //direction.Normalize();  //方向向量化
            
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction*kickForce;        //是一个向量，自带方向

            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
    public void ThrowRock()
    {
        if(attackTarget!=null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);  //复制一份一模一样的东西，生成到场景里。
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
