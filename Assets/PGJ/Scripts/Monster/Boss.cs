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

    Coroutine Co_Attack;

    int shootMaxCnt = 6;
    int shootCnt = 0;
    Boss_Bullet[] shoot_3_Array = new Boss_Bullet[3];

    WaitForSeconds patternIntervalTime = new WaitForSeconds(5);
    WaitForSeconds patternTime = new WaitForSeconds(3);

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(StateTimer());

        shootCnt = 0;

        curtime = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (true == pause || true == isDie)
        {
            return;
        }

        // 박자에 맞춰서 행동
        currentTime += Time.deltaTime;

        if (currentTime >= 30d / gameManager.bpm)
        {
            if (shootCnt < shootMaxCnt)
            {
                Co_Attack = StartCoroutine(AttackProc());
            }

            currentTime -= 30d / gameManager.bpm;
        }

        Quaternion dir = Quaternion.LookRotation(target.transform.position - transform.position);
        Vector3 angle = Quaternion.RotateTowards(transform.rotation, dir, 1200 * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0, angle.y, 0);
        d_angle = Quaternion.Euler(0, dir.eulerAngles.y, 0);
        
        MoveProc();
    }

    IEnumerator StateTimer()
    {
        while (false == isDie)
        {
            yield return patternIntervalTime;

            if (true == isDie)
            {
                break;
            }

            monsterState = MonsterState.Shoot_Attack;

            yield return patternTime;

            shootCnt = 0;

            monsterState = MonsterState.Chase;
        }
    }

    IEnumerator AttackProc()
    {
        switch (monsterState)
        {
            case MonsterState.Shoot_Attack:
                anim.SetBool("Shoot", true);

                yield return null;

                Shoot();

                yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length * 0.66f);
                anim.SetBool("Shoot", false);

                break;

            case MonsterState.Jump_Attack:
                anim.SetBool("jump", true);
                break;

            default:
                anim.SetBool("Shoot", false);
                anim.SetBool("Jump", false);
                break;
        }
    }

    void MoveProc()
    {
        if (MonsterState.Shoot_Attack == monsterState || MonsterState.Jump_Attack == monsterState)
        {
            navAgent.speed = 0;

            anim.SetBool("Run", false);
        }
        else
        {
            if (10 < Vector3.Distance(transform.position, target.transform.position))
            {
                navAgent.SetDestination(target.transform.position);

                navAgent.speed = walkSpeed;

                anim.SetBool("Run", true);
                anim.SetBool("Shoot", false);
                anim.SetBool("Jump", false);
            }
            else
            {
                navAgent.speed = 0;

                anim.SetBool("Run", false);
                anim.SetBool("Shoot", false);
                anim.SetBool("Jump", false);
            }
        }
    }

    protected override void Idle()
    {
        base.Idle();

        //anim.SetBool("Idle", true);
    }

    void Shoot()
    {
        if (shootCnt < shootMaxCnt)
        {
            if (0 == shootCnt % 2)
            {
                SoundManager.Instance.PlaySFX(SFX.BossBulletStartSparkle);    // 처음 발사 소리 삥~
                GameObject go = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                shoot_3_Array[shootCnt / 2] = go.GetComponent<Boss_Bullet>();
            }
            else
            {
                SoundManager.Instance.PlaySFX(SFX.BossBulletShoot);    // 공격 발사 소리 피슝~
                shoot_3_Array[shootCnt / 2].RealShot(target);
            }

            shootCnt++;
        }
    }

    protected override void Die()
    {
        base.Die();
        EventManager.Instance.ClearEvent();
        isDie = true;
    }
}