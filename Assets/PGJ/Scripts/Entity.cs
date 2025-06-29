using System.ComponentModel;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

enum MonsterState
{
    Idle,
    Chase,
    Shoot_Attack,
    Jump_Attack,
    Die
}

public class Entity : MonoBehaviour
{
    [SerializeField] protected int hp;
    [SerializeField] protected int maxHP;
    [SerializeField] protected float walkSpeed;
    [SerializeField] protected float jumpPower;
    [SerializeField] protected bool isFly;

    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Collider[] col;

    [SerializeField] protected Transform groundCheck;        // 바닥 체크 위치
    [SerializeField] protected float groundCheckDistance;    // 바닥 체크 거리
    [SerializeField] protected LayerMask whatIsGround;       // 바닥 레이어

    [SerializeField] protected Transform wallCheck;          // 벽 체크 위치
    [SerializeField] protected float wallCheckDistance;      // 벽 체크 거리
    [SerializeField] protected LayerMask whatIsWall;         // 벽 레이어

    [Space(10)]
    [SerializeField] Image hpImg;

    protected Player target;

    protected GameManager gameManager = null;

    internal MonsterState monsterState;

    protected double currentTime;

    int attackCnt;

    protected bool isDie;

    protected NavMeshAgent navAgent;

    protected float sturnTime;              // 스턴 남은시간

    protected bool pause = false;

    protected virtual void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        Init();
    }

    // 바닥 체크
    //protected bool IsGroundDetected()
    //{
    //    
    //}

    // 벽 체크
    //protected bool IsWallDetected()
    //{

    //}

    protected virtual void Init()
    {
        isDie = false;

        EventManager.Instance.OnPauseAction += SetPause;

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        gameManager = GameManager.Instance;

        monsterState = MonsterState.Idle;
        attackCnt = 0;

        currentTime = 0d;

        hp = maxHP;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnPauseAction -= SetPause;
    }

    void SetPause(bool _isPuase)
    {
        pause = _isPuase;
    }

    protected virtual void Update()
    {
        if (true == pause || true == isDie)
        {
            return;
        }
    }

    internal void GetDamage(int dmg)
    {
        if (true == isDie)
        {
            return;
        }

        hp -= dmg;
        Debug.Log(name + " 체력 : " + hp + " / " + maxHP);
        if (null != hpImg)
        {
            hpImg.fillAmount = (float)hp / maxHP;
        }

        if (hp <= 0 && (MonsterState.Idle == monsterState || MonsterState.Chase == monsterState))
        {
            Die();
        }
    }

    // 움직임 멈춤
    protected virtual void SetZeroVelocity()
    {

    }

    // 추적 이동
    protected virtual void ChaseMove()
    {
        navAgent.SetDestination(target.transform.position);
    }

    // 공격
    protected virtual void Attack()
    {

    }

    // 쉬는 타임
    protected virtual void Idle()
    {

    }

    // 사망
    protected virtual void Die()
    {
        if (null != rb)
        {
            rb.useGravity = false;
        }

        for (int i=0; i<col.Length; i++)
        {
            col[i].enabled = false;
        }

        anim.SetTrigger("Die");
    }

    // 움직임 처리
    //public void SetVelocity(float _x = 0f, float _y = 0f, float _z = 0f)
    //{

    //}

    // 스턴 처리
    //public void SetSturn(float _time)
    //{

    //}

    //protected void OnTriggerEnter()
    //{

    //}

    //protected void Update()
    //{

    //}

    //protected void OnDrawGizmos()
    //{

    //}
}
