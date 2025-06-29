using System.Collections;
using UnityEngine;

public class Boss : Entity
{
    [SerializeField] GameObject bulletPrefab; // 보스의 총알 프리팹
    [SerializeField] Transform bulletSpawnPoint; // 총알이 발사될 위치
    [SerializeField] float bulletSpeed = 10f; // 총알 속도
    [SerializeField] float differ_angle; //
    [SerializeField] float max_time; //

    float curtime;
    Quaternion d_angle;

    WaitForSeconds patternTime = new WaitForSeconds(5);

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(StateTimer());

        curtime = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (isDie)
        {
            return;
        }
        
        Quaternion dir = Quaternion.LookRotation(target.transform.position - transform.position);
        Vector3 angle = Quaternion.RotateTowards(transform.rotation, dir, 1200 * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0, angle.y, 0);
        d_angle = Quaternion.Euler(0, dir.eulerAngles.y, 0);
    }

    IEnumerator StateTimer()
    {
        while (gameObject.activeSelf)
        {
            yield return patternTime;

            switch (Random.Range(0, 2))
            {
                case 0:             // 3연사 패턴
                    break;

                case 1:             // 점프 패턴
                    break;
            }
        }
    }

    protected override void StateProc()
    {
        base.StateProc();

        if (isDie)
        {
            return;
        }

        // 보스의 상태에 따라 행동
        switch (monsterState)
        {
            case MonsterState.Idle:
                Idle();
                break;

            case MonsterState.Chase:
                // 플레이어를 추적
                Chase();
                break;

            case MonsterState.Attack:
                // 플레이어에게 총알 발사
                Shoot();
                break;

            case MonsterState.Die:
                // 죽음 처리
                Die();
                break;
        }
    }

    protected override void Idle()
    {
        base.Idle();

        //anim.SetBool("Idle", true);
    }

    void Chase()
    {
        navAgent.SetDestination(target.transform.position);
        if (Vector3.Distance(transform.position, target.transform.position) < 20f)
        {
            monsterState = MonsterState.Attack;
        }
    }

    void Shoot()
    {
        int attackCount = 3;

        if (0 < attackCount)
        {
            attackCount--;

            if (Quaternion.Angle(transform.rotation, d_angle) < differ_angle)
            {
                curtime -= Time.deltaTime;
                if (curtime <= 0)
                {
                    GameObject go = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    //go.
                    curtime = max_time;
                }
            }
        }
    }

    protected override void Die()
    {
        base.Die();

        isDie = true;
    }
}