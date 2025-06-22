using UnityEngine;

public class Monster : MonoBehaviour
{
    private Animator anim;
    private CharacterController character;
    private Transform player;

    [Header("이동")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;
    public float moveTime = 2f;
    public float waitTime = 1f;
    private float timer;
    private bool isWaiting = false;
    private bool isRunning = false;
    private Vector3 moveDirection;

    [Header("감지")]
    public float detectRange = 5f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("체력")]
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float currentHp;

    private void Start()
    {
        anim = GetComponent<Animator>();
        character = GetComponent<CharacterController>();

        timer = moveTime;
        moveDirection = transform.forward;

        currentHp = maxHp;
    }

    private void Update()
    {
        DetectPlayer();
        Move();
    }

    #region 이동
    private void Move()
    {
        float currentSpeed = 0f;

        if (isRunning)
        {
            character.Move(moveDirection * runSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(moveDirection);
            currentSpeed = runSpeed;
        }
        else
        {
            if (isWaiting)
            {
                timer -= Time.deltaTime;
                currentSpeed = 0f;
                if (timer <= 0f)
                {
                    isWaiting = false;
                    timer = moveTime;
                }
            }
            else
            {
                character.Move(moveDirection * walkSpeed * Time.deltaTime);
                currentSpeed = walkSpeed;

                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isWaiting = true;
                    timer = waitTime;
                    currentSpeed = 0f;
                }

                if (IsObstacle() || !IsGround())
                {
                    Turn();
                }
            }
        }

        anim.SetFloat("Speed", currentSpeed);
    }

    private void Turn()
    {
        int[] angles = { -90, 90, 180 };
        int angle = angles[Random.Range(0, angles.Length)];

        transform.Rotate(0, angle, 0);
        moveDirection = transform.forward;
    }
    #endregion

    #region 감지
    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;
            isRunning = true;
            moveDirection = (player.position - transform.position).normalized;
        }
        else
        {
            isRunning = false;
        }
    }

    private bool IsObstacle()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, moveDirection);
        return Physics.Raycast(ray, 0.6f, obstacleLayer);
    }

    private bool IsGround()
    {
        Vector3 origin = transform.position + moveDirection * 0.5f + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
    #endregion

    public void Hit(float damage)
    {
        currentHp -= damage;


        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }

        if (currentHp <= 0)
        {
            Debug.Log("사망");
        }
    }
}
