using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates {HitPlayer ,HitEnemy,HitNothing} //枚举
    public RockStates rockStates;              //枚举变量记录状态

    private Rigidbody rb;   //刚体
    [Header("Basic Setting")]
    public float force;     //向前冲向的力的大小
    public GameObject target;

    public int damage;      //石头的伤害

    private Vector3 direction;

    public GameObject breakEffect;  //石头破碎参数

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;  //石头的初始速度改为1，跳过一开始的石头状态的判断

        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        if(target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = ((target.transform.position - transform.position)+Vector3.up).normalized;  //加上Vector3.up是因为想有个向上的力，让石头在空中多呆一会
        rb.AddForce(direction * force, ForceMode.Impulse);

    }

    private void OnCollisionEnter(Collision other)
    {
        //Debug.Log($"碰到了: {other.gameObject.name}，当前状态: {rockStates}"); // 加这行
        switch (rockStates)
        {
            case RockStates.HitPlayer :
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity=direction*force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                
                    rockStates = RockStates.HitNothing;

                }
                break;
            case RockStates.HitEnemy :
                if(other.gameObject.GetComponent<Golem>())
                {
                    Debug.Log($"碰到了: {other.gameObject.name}，当前状态: {rockStates}"); // 加这行
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                //Destroy(gameObject);
                break;
        }
    }

}
