using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class Weapon_ShotGun : WeaponBase
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    //Animator animator; // 애니메이터 컴포넌트
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();
    [SerializeField] List<GameObject> hitEffect = new();
    [SerializeField] AudioSource audio;
    Player player;

    Dictionary<string, AnimationClip> animDic = new();

    protected override void Awake()
    {
        base.Awake(); // 부모 클래스의 Awake 호출
        //animator = GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트 초기화
        if (audio == null)
        {
            audio = GetComponent<AudioSource>(); // 오디오 소스 초기화
            if(audio == null)
                audio = gameObject.AddComponent<AudioSource>(); // 오디오 소스가 없으면 추가
        }

        player = transform.parent.GetComponentInParent<Player>();

    }

    private void OnEnable()
    {
        if (audio == null)
        {
            audio = GetComponent<AudioSource>(); // 오디오 소스 초기화
            if (audio == null)
                audio = gameObject.AddComponent<AudioSource>(); // 오디오 소스가 없으면 추가
        }
        audio.PlayOneShot(gunSound[4], 1f);
    }

    private void OnDisable()
    {
        audio.Stop(); // 오디오 소스 정지
        FindAnyObjectByType<AudioSource>()?.PlayOneShot(gunSound[4], .5f); // 다른 오디오 소스에서 사운드 재생
    }

    protected override void Update()
    {
        //base.Update();
        gameManager = gameManager ?? GameManager.Instance; // 게임 매니저 인스턴스 재설정
        rhythmTimingNum = gameManager.RhythmCheck();

        //Debug.Log($"input r : {inputManager.r_Input}");
        //Debug.Log($"mouse 0 : {inputManager.mouse0_Input}");
        //if (inputManager.r_Input)
        if(input.r_Input)
        {
            input.r_Input = false;

            Reload();
            //inputManager.r_Input = false; // 재장전 입력 초기화
        }

        else if(input.mouse0_Input)
        {
            input.mouse0_Input = false;

            Shoot();
            //inputManager.mouse0_Input = false; // 발사 입력 초기화
        }
    }

    protected override void Reload()
    {
        if (nowAmmo == maxAmmo)
        {
            Debug.Log("재장전이 필요 없습니다!"); // 재장전이 필요 없으면 에러 로그 출력
            return; 
        }
        if(anim.GetBool("Reload") == true)
        {
            Debug.Log("이미 재장전 중입니다!"); // 이미 재장전 중이면 에러 로그 출력
            return;
        }

        int beat = gameManager.RhythmCheck(); // 현재 박자 체크
        Debug.Log($"Beat: {beat}"); // 현재 박자 로그 출력

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

        audio.PlayOneShot(gunSound[beat == 0 ? 2 : 3]);
        anim.SetBool("Reload", true); // 애니메이터 트리거 설정
        

        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.speed = beat == 0 ? 2 : 1;

        float delay = (animDic["Reload"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // 애니메이션 딜레이 계산
        StartCoroutine(ActionDelay(() =>
        {
            anim.SetBool("Reload", false);
            anim.speed = 1; // 애니메이션 속도 초기화
        }, delay)); // 재장전 애니메이션 딜레이
        //base.Reload();
    }

    protected override void Shoot()
    {
        if(anim.GetBool("Reload") == true)
        {
            Debug.Log("재장전 중에는 발사할 수 없습니다!"); // 재장전 중이면 에러 로그 출력
            return;
        }
        if (anim.GetBool("Fire") == true)
        {
            Debug.Log("이미 발사 중입니다!"); // 이미 발사 중이면 에러 로그 출력
            return;
        }
        if (nowAmmo <= 0)
        {
            Debug.Log("발사 준비된 탄환이 없습니다!"); // 발사 준비된 탄환이 없으면 에러 로그 출력
            return;            
        }

        int beat = gameManager.RhythmCheck(); // 현재 박자 체크
        Debug.Log($"Beat: {beat}"); // 현재 박자 로그 출력

        gameManager.NotePush();

        nowAmmo--;                          // 발사 준비된 탄환 감소
        //base.Shoot();

        StartCoroutine(Fire(beat));
    }



    IEnumerator Fire(int beat)
    {
        audio.PlayOneShot(gunSound[beat == 0 ? 1 : 0]); // 발사 사운드 재생
        anim.SetBool("Fire", true); // 애니메이터 트리거 설정
        Transform trf = Camera.main.transform;

        //beat == 0 ? gunSound[0] : gunSound[1]

        if (animDic.ContainsKey("Fire") == false)
        {
            AnimationClip clip = GetAnimClip("Fire", false);
            if (clip != null)
                animDic.Add("Fire", clip);
        }

        anim.speed = beat == 0 ? 2f : 1f; // 애니메이션 속도 설정

        Vector3 pos = shootPoint.position;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        WaitForSeconds wait = new(0.1f);
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * trf.forward; // 퍼짐 방향 계산
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.LookRotation(v));
            bullet.transform.forward = v;
            Bullet b = bullet.GetComponent<Bullet>();
            if(b != null)
            {
                b.Set(_position: pos, _lotation: Quaternion.LookRotation(v), _direction: dir, _speed: bulletSpeed,
                    _damage: beat == 0 ? 4 : beat == 1 ? 2 : 1, _weapon: this,
                    _hitObj: beat == 0 ? hitEffect[1].GetComponent<HitEffectObj>() : hitEffect[0].GetComponent<HitEffectObj>()//,
                    //_shooter: player
                    );
            }
            //yield return waitForFixedUpdate; // 프레임 딜레이
            //yield return waitForFixedUpdate;
            yield return wait;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        float delay = (animDic["Fire"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // 애니메이션 딜레이 계산
        StartCoroutine(ActionDelay(() =>
        {
            anim.SetBool("Fire", false);
            anim.speed = 1f;
        }, delay)); // 재장전 애니메이션 딜레이
    }

    Quaternion GetSpreadDirection()
    {
        // 기본 방향
        //Vector3 forward = shootPoint.forward;

        // 퍼짐 적용
        //float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        //float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);
        var x = UnityEngine.Random.Range(-shotSpread, shotSpread);
        var y = UnityEngine.Random.Range(-shotSpread, shotSpread);

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
        var clips = anim.runtimeAnimatorController.animationClips;
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
