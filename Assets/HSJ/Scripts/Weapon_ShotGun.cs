using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon_ShotGun : WeaponBase
{
    InputManager inputManager; // 입력 매니저 인스턴스
    GameManager gameManager; // 게임 매니저 인스턴스
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    Animator animator; // 애니메이터 컴포넌트
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();
    [SerializeField] List<GameObject> hitEffect = new();

    Dictionary<string, AnimationClip> animDic = new();

    private void Start()
    {
        gameManager = GameManager.Instance; // 게임 매니저 인스턴스 초기화
        inputManager = InputManager.Instance; // 입력 매니저 인스턴스 초기화
        animator = GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트 초기화
    }

    protected override void Update()
    {
        //base.Update();
        rhythmTimingNum = gameManager.RhythmCheck();

        if (inputManager.r_Input)
        {
            Reload();
            inputManager.r_Input = false; // 재장전 입력 초기화
        }

        else if(inputManager.mouse0_Input)
        {
            Shoot();
            inputManager.mouse0_Input = false; // 발사 입력 초기화
        }
    }

    protected override void Reload()
    {
        if (nowAmmo >= maxAmmo)
        {
            Debug.LogError("재장전이 필요 없습니다!"); // 재장전이 필요 없으면 에러 로그 출력
            return; 
        }
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("이미 재장전 중입니다!"); // 이미 재장전 중이면 에러 로그 출력
            return;
        }

        int beat = gameManager.RhythmCheck(); // 현재 박자 체크

        //if (currentReloadStepNum >= reloadStepNum)
        //{
        //    return; // 현재 장전 단계가 최대 단계에 도달했으면 리턴
        //}

        gameManager.NotePush(); // 노트 푸시 호출
        nowAmmo += 1; // 발사 준비된 탄환 증가

        if(animDic.ContainsKey("Reload") == false)
        {
            AnimationClip clip = GetAnimClip("Reload", false);
            if (clip != null)
                animDic.Add("Reload", clip);
            
        }

        animator.SetBool("Reload", true); // 애니메이터 트리거 설정
        

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        animator.speed = beat == 0 ? 2 : 1;

        float delay = (animDic["Reload"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // 애니메이션 딜레이 계산
        StartCoroutine(ActionDelay(() =>
        {
            animator.SetBool("Reload", false);
            animator.speed = 1; // 애니메이션 속도 초기화
        }, delay)); // 재장전 애니메이션 딜레이
        //base.Reload();
    }

    protected override void Shoot()
    {
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("재장전 중에는 발사할 수 없습니다!"); // 재장전 중이면 에러 로그 출력
            return;
        }
        if (animator.GetBool("Fire") == true)
        {
            Debug.LogError("이미 발사 중입니다!"); // 이미 발사 중이면 에러 로그 출력
            return;
        }
        if (nowAmmo <= 0)
        {
            Debug.LogError("발사 준비된 탄환이 없습니다!"); // 발사 준비된 탄환이 없으면 에러 로그 출력
            return;            
        }

        int beat = gameManager.RhythmCheck(); // 현재 박자 체크

        gameManager.NotePush();

        nowAmmo--;                          // 발사 준비된 탄환 감소
        //base.Shoot();

        StartCoroutine(Fire(beat));
    }



    IEnumerator Fire(int beat)
    {

        animator.SetBool("Fire", true); // 애니메이터 트리거 설정
        Transform trf = Camera.main.transform;

        //beat == 0 ? gunSound[0] : gunSound[1]

        if (animDic.ContainsKey("Fire") == false)
        {
            AnimationClip clip = GetAnimClip("Fire", false);
            if (clip != null)
                animDic.Add("Fire", clip);
        }

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * Camera.main.transform.forward; // 퍼짐 방향 계산
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(v));
            bullet.transform.localScale = bulletScale; // 크기 설정
            bullet.GetComponent<Rigidbody>().linearVelocity = v * bulletSpeed;
            Bullet b = bullet.GetComponent<Bullet>();
            if(b != null)
            {
                b.Set(shootPoint.position, Quaternion.LookRotation(v), dir, bulletSpeed,
                    bulletSpeed, beat == 0 ? 4 : beat == 1 ? 2 : 1, this,
                    _hitObj: beat == 0 ? hitEffect[0].GetComponent<HitEffectObj>() : hitEffect[1].GetComponent<HitEffectObj>());
            }
            //yield return waitForFixedUpdate; // 프레임 딜레이
            yield return waitForFixedUpdate;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        float delay = (animDic["Fire"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // 애니메이션 딜레이 계산
        StartCoroutine(ActionDelay(() => animator.SetBool("Fire", false), delay)); // 재장전 애니메이션 딜레이
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

    private AnimationClip GetAnimClip(string name, bool isMach = true)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (isMach && clip.name == name)
                return clip;
            if(!isMach && clip.name.Contains(name))
                return clip;
        }
        return null;
    }
}
