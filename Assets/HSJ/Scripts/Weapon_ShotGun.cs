using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon_ShotGun : WeaponBase
{
    GameManager gameManager; // 게임 매니저 인스턴스
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    Animator animator; // 애니메이터 컴포넌트
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();

    private void Start()
    {
        gameManager = GameManager.Instance; // 게임 매니저 인스턴스 초기화
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 초기화
    }

    protected override void Update()
    {
        //base.Update();
        rhythmTimingNum = gameManager.RhythmCheck();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        else if(Input.GetMouseButtonDown(0))
            Shoot();
    }

    protected override void Reload()
    {
        if (nowAmmo >= maxAmmo)
        {
            Debug.LogError("재장전이 필요 없습니다!"); // 재장전이 필요 없으면 에러 로그 출력
            return; 
        }
        if (gameManager.RhythmCheck() == 0)
        {
            Debug.LogError("박자 타이밍이 아닙니다!"); // 박자 타이밍이 아니면 에러 로그 출력
            return; 
        }
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("이미 재장전 중입니다!"); // 이미 재장전 중이면 에러 로그 출력
            return;
        }

        //if (currentReloadStepNum >= reloadStepNum)
        //{
        //    return; // 현재 장전 단계가 최대 단계에 도달했으면 리턴
        //}
        gameManager.NotePush(); // 노트 푸시 호출
        nowAmmo += 1; // 발사 준비된 탄환 증가
        animator.SetBool("Reload", true); // 애니메이터 트리거 설정
        StartCoroutine(ActionDelay(() => animator.SetBool("Reload", false), 0.5f)); // 재장전 애니메이션 딜레이
        //base.Reload();
    }

    protected override void Shoot()
    {
        //TODO 1. RhythmCheck() 0 으로 계속 옴
        //TODO 2. base.Shoot() 에서 return시 아래 코드가 실행됨
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("재장전 중에는 발사할 수 없습니다!"); // 재장전 중이면 에러 로그 출력
            return;
        }   

        Debug.Log($"RhythmCheck : {gameManager.RhythmCheck()}");
        //Debug.Log(string.Format("rhythmTimingNum : {0}", rhythmTimingNum));
        //if (Time.time < nextShotTime) return; // 딜레이 시간 체크
        if (nowAmmo <= 0)
        {
            Debug.LogError("발사 준비된 탄환이 없습니다!"); // 발사 준비된 탄환이 없으면 에러 로그 출력
            return;            
        }

        if(animator.GetBool("Fire") == true)
        {
            Debug.LogError("이미 발사 중입니다!"); // 이미 발사 중이면 에러 로그 출력
            return;
        }

        gameManager.NotePush();

        if (rhythmTimingNum == 0)
        {
            Debug.LogError("박자 타이밍이 아닙니다!"); // 박자 타이밍이 아니면 에러 로그 출력
            return;   // 박자 타이밍이 아니면 리턴
        }

        nowAmmo--;                          // 발사 준비된 탄환 감소
        //base.Shoot();

        StartCoroutine(Fire());
    }



    IEnumerator Fire()
    {

        animator.SetBool("Fire", true); // 애니메이터 트리거 설정
        Transform trf = Camera.main.transform;
        

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * Camera.main.transform.forward; // 퍼짐 방향 계산
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(v));
            bullet.transform.localScale = bulletScale; // 크기 설정
            bullet.GetComponent<Rigidbody>().linearVelocity = v * bulletSpeed;
            //yield return waitForFixedUpdate; // 프레임 딜레이
            yield return null;
        }

        animator.SetBool("Fire", false); // 발사 애니메이션 종료
    }

    Quaternion GetSpreadDirection()
    {
        // 기본 방향
        //Vector3 forward = shootPoint.forward;

        // 퍼짐 적용
        //float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        //float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);
        var x = Random.Range(-shotSpread, shotSpread);
        var y = Random.Range(-shotSpread, shotSpread);

        // 로테이션을 만들고 회전 적용
        return Quaternion.Euler(y, x, 0f);
        //Quaternion spreadRotation = Quaternion.Euler(y, x, 0);
        //return spreadRotation * forward;
    }

    IEnumerator ActionDelay(UnityAction action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // 딜레이 시간 대기
        action?.Invoke(); // 액션 호출
    }

    public void ObjectHit()
    {

    }
}
