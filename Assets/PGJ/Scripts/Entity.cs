using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.AI;

enum MonsterState
{
    Idle,
    Chase,
    Attack,
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
    [SerializeField] protected Collider col;

    [SerializeField] protected Transform groundCheck;        // 바닥 체크 위치
    [SerializeField] protected float groundCheckDistance;    // 바닥 체크 거리
    [SerializeField] protected LayerMask whatIsGround;       // 바닥 레이어

    [SerializeField] protected Transform wallCheck;          // 벽 체크 위치
    [SerializeField] protected float wallCheckDistance;      // 벽 체크 거리
    [SerializeField] protected LayerMask whatIsWall;         // 벽 레이어

    Player target;

    GameManager gameManager = null;

    internal MonsterState monsterState;

    double currentTime;

    int attackCnt;

    protected NavMeshAgent navAgent;

    protected float sturnTime;              // 스턴 남은시간

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
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        gameManager = GameManager.Instance;

        monsterState = MonsterState.Chase;
        attackCnt = 0;

        currentTime = 0d;
    }

    protected virtual void Update()
    {
        // 박자에 맞춰서 행동
        currentTime += Time.deltaTime;

        if (currentTime >= 30d / gameManager.bpm)
        {
            StateProc();

            currentTime -= 30d / gameManager.bpm;
        }
    }

    // 행동 양식
    protected virtual void StateProc()
    {
        switch (monsterState)
        {
            case MonsterState.Idle:
                break;

            case MonsterState.Chase:
                break;

            case MonsterState.Attack:
                break;

            case MonsterState.Die:
                break;
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
