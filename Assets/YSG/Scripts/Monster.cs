using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private Animator anim;
    private CharacterController character;
    private NavMeshAgent agent;
    private Transform target;

    private float timer;

    private bool isWaiting = false;
    private bool isRunning = false;
    private bool isAttacking = false;
    private bool isHit = false;
    private bool isDead = false;

    [Header("�̵�")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4;
    public float moveTime = 5;
    public float waitTime = 1;
    private Vector3 moveDirection;

    [Header("����")]
    [SerializeField] private float detectRange = 5;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;

    [Header("����")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Transform attackCenter;
    [SerializeField] private float attackRadius = 1;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackTimer = 0;

    [Header("ü��")]
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float currentHp;

    private void Start()
    {
        anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        character = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.enabled = false;

        timer = moveTime;
        moveDirection = transform.forward;
        currentHp = maxHp;
    }

    private void Update()
    {
        if (isDead) return;

        if (isHit)
        {
            anim.SetFloat("Speed", 0);
            return;
        }

        DetectPlayer();

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (target != null && IsInAttackRange() && !isAttacking)
            Attack();

        if (isAttacking)
        {
            if (agent.enabled) agent.enabled = false;

            anim.SetFloat("Speed", 0);
            return;
        }

        if (target != null) isRunning = true;

        if (isRunning)
            Run();
        else
            Walk();
    }

    #region �̵�
    private void Walk()
    {
        if (agent.enabled) agent.enabled = false;

        float currentSpeed = 0;

        if (isWaiting)
        {
            timer -= Time.deltaTime;
            currentSpeed = 0;
            if (timer < 0)
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
            if (timer < 0)
            {
                isWaiting = true;
                timer = waitTime;
                currentSpeed = 0;
            }

            if (IsObstacle() || !IsGround())
                Turn();
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

    private void Run()
    {
        if (!agent.enabled) agent.enabled = true;

        agent.speed = runSpeed;
        agent.SetDestination(target.position);

        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 dir = agent.desiredVelocity.normalized;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }

            anim.SetFloat("Speed", runSpeed);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }
    #endregion

    #region ����
    private void DetectPlayer()
    {
        if (target != null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                target = hit.transform;
                break;
            }
        }
    }

    private bool IsInAttackRange()
    {
        Collider[] hits = Physics.OverlapSphere(attackCenter.position, attackRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) return true;
        }
        return false;
    }

    private bool IsObstacle()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, moveDirection);
        return Physics.Raycast(ray, 0.6f, groundLayer);
    }

    private bool IsGround()
    {
        Vector3 origin = transform.position + moveDirection * 0.5f + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 1, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        if (attackCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCenter.position, attackRadius);
        }
    }
    #endregion

    #region ����
    private void Attack()
    {
        if (attackTimer > 0) return;

        isAttacking = true;

        anim?.SetTrigger("Attack");

        if (target == null)
        {
            GameObject goPlayer = GameObject.FindGameObjectWithTag("Player");
            if (goPlayer != null)
            {
                this.target = goPlayer.transform;
            }
        }


        if (target.TryGetComponent(out Player player))
        {
            player.GetDamage(attackDamage);
        }

        Debug.Log($"�÷��̾� {attackDamage} ������ > �÷��̾� ü�� : {target.GetComponent<Player>().currentHp}");

        attackTimer = attackCooldown;
    }

    public void Hit(float damage)
    {
        currentHp -= damage;

        anim?.SetTrigger("Hit");

        isHit = true;
        isRunning = true;

        if (target == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null)
            {
                this.target = target.transform;
            }
        }

        if (currentHp <= 0)
        {
            Debug.Log("���");

            Die();
        }
    }

    public void EndAttack() => isAttacking = false;
    public void EndHit() => isHit = false;

    private void Die()
    {
        anim?.SetTrigger("Death");

        isAttacking = false;
        isHit = false;
        isRunning = false;
        isDead = true;

        if (agent != null) agent.enabled = false;
        if (character != null) character.enabled = false;
    }

    #endregion
}
