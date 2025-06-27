using System;
using System.Numerics;
using Unity.Collections;
using UnityEngine;

public class Boss : Entity
{
    [SerializeField] GameObject player; // 플레이어 타겟
    [SerializeField] Transform target;

    [SerializeField] GameObject bulletPrefab; // 보스의 총알 프리팹
    [SerializeField] Transform bulletSpawnPoint; // 총알이 발사될 위치
    [SerializeField] float bulletSpeed = 10f; // 총알 속도
    [SerializeField] float differ_angle; //
    [SerializeField] float curtime; //
    [SerializeField] float max_time; //


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        // 보스의 초기 상태 설정
        // monsterState = MonsterState.Idle;
    }

    protected override void Update()
    {
        base.Update();

        if (target == null)
        {
            player.transform.Rotate(new UnityEngine.Vector3(0, 60, 0) * Time.deltaTime);
        }
        else
        {
            UnityEngine.Quaternion dir = UnityEngine.Quaternion.LookRotation(target.position - transform.position);
            UnityEngine.Vector3 angle = UnityEngine.Quaternion.RotateTowards(transform.rotation, dir, 200 * Time.deltaTime).eulerAngles;
            transform.rotation = UnityEngine.Quaternion.Euler(0, angle.y, 0);
            UnityEngine.Quaternion d_angle = UnityEngine.Quaternion.Euler(0, dir.eulerAngles.y, 0);

            if (UnityEngine.Quaternion.Angle(transform.rotation, d_angle) < differ_angle)
            {
                curtime -= Time.deltaTime;
                if (curtime <= 0)
                {
                    var a = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    a.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint.transform.forward * bulletSpeed);
                    Destroy(a, 2f);
                    curtime = max_time;
                }

                
            }

        }

        // 보스의 상태에 따라 행동
        // switch (monsterState)
        // {
        //     case MonsterState.Idle:
        //         // 대기 상태에서 플레이어를 감지하면 추적 상태로 전환
        //         if (target != null && Vector3.Distance(transform.position, target.transform.position) < 10f)
        //         {
        //             monsterState = MonsterState.Chase;
        //         }
        //         break;

            //     case MonsterState.Chase:
            //         // 플레이어를 추적
            //         navAgent.SetDestination(target.transform.position);
            //         if (Vector3.Distance(transform.position, target.transform.position) < 2f)
            //         {
            //             monsterState = MonsterState.Attack;
            //         }
            //         break;

            //     case MonsterState.Attack:
            //         // 플레이어에게 총알 발사
            //         Shoot();
            //         break;

            //     case MonsterState.Die:
            //         // 죽음 처리
            //         Die();
            //         break;
            // }
    }

    private void Destroy(GameObject a, GameObject gameObject, float v)
    {
        throw new NotImplementedException();
    }
}