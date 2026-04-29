using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("skill")]
    public float kickForce = 10;

    public void kickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            Vector3 direction = attackTarget.transform.position-transform.position;  //两个向量相减

            direction.Normalize();          //向量单位化

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;     //被攻击玩家的移动被打断

            attackTarget.GetComponent<NavMeshAgent>().velocity = direction*kickForce;

            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
