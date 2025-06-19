using UnityEngine;

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

    protected float sturnTime;              // 스턴 남은시간

    protected virtual void Awake()
    {
        
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

    // 움직임 멈춤
    //public void SetZeroVelocity()
    //{
    
    //}

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
