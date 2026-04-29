using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}  //守卫 巡逻 追逐 死亡
[RequireComponent(typeof(NavMeshAgent))]            //拖拽在人物上会自动出现NavMeshAgen组件
[RequireComponent (typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    public EnemyStates enemyStates;
    private NavMeshAgent agent;     //自动寻路组件

    protected CharacterStats characterStats; //敌人数值

    [Header("Basic Setting")]
    public float sightRadius;       //发现玩家范围

    protected GameObject attackTarget;    //敌人攻击目标

    public float lookAtTime;            //敌人观察的时间

    private float remainLookAtTime;     //剩余的观察时间

    private float lastAttackTime;       //攻击间隔

    public bool isGuard;        //敌人是否为站桩敌人

    private float speed;        //记录敌人的原始速度

    [Header("Patrol State")]

    public float patrolRange;
    private Vector3 wayPoint;

    //bool配合动画
    bool isWalk;        //是否行走

    bool isChase;       //是否追赶

    bool isFollow;      //是否跟随
    
    bool isDeah;        //是否死亡

    private Animator anim;          //敌人动画插件

    private Collider coll;

    private Vector3 guardPos;        //记录敌人的初始位置

    private Quaternion guardRotation;   //记录敌人的旋转角度

    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        guardPos = agent.transform.position;
        speed = agent.speed;
        remainLookAtTime = lookAtTime;
        guardRotation = transform.rotation;
    }
    void Start()
    {
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //FIXME: 场景切换后修改
        GameManager.Instance.AddObserver(this);
    }

    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }
    void Update()
    {
        if (characterStats.CurrentHealth <= 0)
            isDeah = true;
        if (!playerDead)
        {
            SwithStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    //设置动画啊
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Deadth", isDeah);

    }
    void SwithStates()
    {

        if(isDeah)
            enemyStates = EnemyStates.DEAD;
        //如果发现player 切换到CHASE
        else if(FoundPlayer())
        {
            enemyStates= EnemyStates.CHASE;
        }
        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(transform.position - guardPos) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.5f); //Lerp是缓慢运行，（原位置，目标位置，转的速度（0-1））
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase= false;
                agent.speed = speed*0.5f;       //乘法比除法性能好
                if(Vector3.Distance(wayPoint,transform.position)<=agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                }else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                    break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        //agent.destination = guardPos;
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        //agent.destination = guardPos;
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if(lastAttackTime <= 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                //agent.enabled = false;  //敌人停止移动
                agent.radius = 0;
                coll.enabled = false;   //碰撞体取消
                Destroy(gameObject,2f);  //2f:延迟2秒
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach(var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStats.attackData.attackRange;
        }
        else return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else return false;
    }

    void GetNewWayPoint()   //巡逻的路线
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange,patrolRange);
        //第二个不改是因为有坡
        Vector3 randomPoint = new Vector3(guardPos.x+randomX, transform.position.y, guardPos.z+randomZ);

        //wayPoint = randomPoint;     //可能生成的位置在障碍物上

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    void OnDrawGizmosSelected()   //给监视范围画出来
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        anim.SetBool("Win", true);
        playerDead = true;
        //停止所有移动
        //停止Agent
        isChase = false;
        isWalk = false; 
        attackTarget = null;
    }
}
