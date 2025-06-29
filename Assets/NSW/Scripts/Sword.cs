using UnityEngine;
using System.Collections.Generic;

public class Sword : WeaponBase
{
    // 장전 상태를 관리하기 위한 열거형
    private enum ReloadStage { None, Stage1_Raising, Stage1_Complete, Stage2_Ready, Success, Fail }
    private ReloadStage currentReloadStage = ReloadStage.None;
    private SwordDrawSound swordDrawSound; 

    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;    // Draw Sword 2 애니 길이
    [SerializeField] float sparkleDuration = 0.3f;    // 파티클 유지 시간
    [SerializeField] float reloadLowerDuration = 0.5f;    // Put 애니 길이

    [Header("Attack Settings")]
    //[Tooltip("공격 애니메이션 속도")]
    //[SerializeField] float attackAnimationSpeed = 1f;
    [Tooltip("공격 애니메이션 실제 길이 (초)")]
    [SerializeField] float attackDuration = 0.18f; 
    // [Tooltip("공격 쿨다운 (초)")] // 더 이상 직접적인 쿨다운으로 사용되지 않습니다.
    [SerializeField] float attackCooldown = 0.8f; // 이 변수 자체는 남아있지만 코드에서는 공격 쿨다운으로 사용되지 않습니다.

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Reload Settings")]
    //[Tooltip("재장전 후 회복될 내구도(탄환)량")]
    //[SerializeField] int reloadShot = 100; // 누락되었던 reloadShot 변수 재추가
    [Tooltip("1단계 장전 후 다음 R키 입력까지 대기 시간 (타임아웃)")]
    [SerializeField] float stage1InputTimeout = 2.0f; // 초기값 2.0으로 되돌려 놓음. 필요시 Inspector에서 5.0 등으로 변경 가능.

    Transform cameraTr;

    // 장전 중 중복 입력 방지를 위한 플래그
    bool isReloadingInputBlocked = false;
    // 리듬 입력 대기 중인지 여부 (정확한 타이밍에 R키를 눌러야 할 때 사용)
    bool awaitingRhythmInput = false;


    protected override void Awake()
    {
        base.Awake(); // WeaponBase의 Awake 호출

        cameraTr = Camera.main.transform;

        // Sparkle 프리팹 인스턴스 생성 및 초기화
        foreach (var prefab in sparklePrefabs)
        {
            if (prefab == null) continue;
            var inst = Instantiate(prefab, transform);
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sparkleInstances.Add(inst);
        }

    }

    protected override void Reload()
    {
        // 탄 꽉참
        if (nowAmmo == maxAmmo)
        {
            return;
        }

        HandleReloadInput();

        gameManager.NotePush();
    }

    protected override void Update()
    {
        base.Update();

        if (input.mouse0_Input)        // 공격
        {
            input.mouse0_Input = false;

            WeaponAttack();
        }
        else if (input.r_Input)         // 재장전
        {
            input.r_Input = false;

            Reload();
        }

        //base.Update();

        // 0번 키로 Draw (DrawSwordSequence 코루틴이 주석 처리되어 현재 작동하지 않습니다.)
        // KeyCode drawKey = KeyCode.Alpha0 + weaponNum;
        // if (!isDrawing && Input.GetKeyDown(drawKey) && currentReloadStage == ReloadStage.None)
        // {
        //     StartCoroutine(DrawSwordSequence());
        // }

        // R 키로 Reload (장전 입력 블록 중이 아닐 때만 허용)
        //if (!isReloadingInputBlocked && input.r_Input) // <-- 이 부분을 다시 추가
        //{
        //    input.r_Input = false; // 입력 소비

        //    HandleReloadInput();

        //    gameManager.NotePush();
        //}

        // 좌클릭으로 Attack
        // 이전에 마우스 공격 문제를 해결하기 위해 수정했던 부분입니다.
        // 다시 input.mouse0_Input으로 되돌려 놓습니다.
        //if (input.mouse0_Input) // <-- 이 줄로 변경
        //{
        //     input.mouse0_Input = false;
        //    //Debug.Log("[InputDebug] WeaponAttack() called via Input.GetMouseButtonDown(0).");

        //    WeaponAttack();
        //}
    }


    void HandleReloadInput()
    {
        if (!reloading)
        {
            reloading = true;
            currentReloadStepNum = 1;

            anim.SetTrigger("RaiseSword");

            sparkleInstances[0].Clear();
            sparkleInstances[0].Play();

            if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
            {
                gameManager.AddCombo();
                SoundManager.Instance.PlaySFX(SFX.SwordDraw);
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                gameManager.SetHalfCombo();
            }
        }
        else if (currentReloadStepNum < 2)
        {
            currentReloadStepNum++;

            anim.SetTrigger("Reload");
            sparkleInstances[1].Clear();
            sparkleInstances[1].Play();

            if (2 == currentReloadStepNum)
            {
                if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
                {
                    gameManager.AddCombo();
                    SoundManager.Instance.PlaySFX(SFX.SwordDraw);
                }
                else
                {
                    Debug.Log("박자 타이밍 실패...");
                    gameManager.SetHalfCombo();
                }

                nowAmmo = maxAmmo;
                EventManager.Instance.PlayerCurrentBulletUIRefresh(nowAmmo);
                currentReloadStepNum = 0;
                reloading = false;
            }
        }

        //switch (currentReloadStage)
        //{
        //    case ReloadStage.None: // 초기 상태: 첫 R키 입력 -> 1단계 장전 시작
        //        StartCoroutine(ReloadStage1Sequence());
        //        break;

        //    case ReloadStage.Stage1_Complete: // 1단계 완료: 두 번째 R키 입력 대기 중
        //        //awaitingRhythmInput이 true일 때만 R키 입력이 유효하도록
        //        if (awaitingRhythmInput)
        //        {
        //            awaitingRhythmInput = false; // R키 입력 받았으니 대기 상태 해제
        //            StartCoroutine(ReloadStage2Sequence());
        //        }
        //        else
        //        {
        //            // 이미 타임아웃 되었거나, 아직 1단계 애니메이션 중일 수 있음.
        //            // 이 경우, 플레이어에게 피드백을 주거나,
        //            // 무효한 입력으로 간주하고 현재 장전 상태를 실패로 돌릴 수도 있습니다.
        //            StartCoroutine(ReloadFailSequence()); // 즉시 실패 처리하는 경우
        //        }
        //        break;

        //    case ReloadStage.Stage1_Raising: // 1단계 애니메이션이 재생 중일 때 R키 입력
        //        //Debug.Log("장전 1단계 애니메이션 중입니다. 잠시 기다려주세요.");
        //        // 이 상황에서 R키를 다시 누르면 씹히거나, 처음부터 다시 시작하게 할 수도 있습니다.
        //        break;

        //    case ReloadStage.Success: // 장전 성공 후: 추가 R키 입력 무시
        //    case ReloadStage.Fail: // 장전 실패 후: 추가 R키 입력 무시
        //        //Debug.Log("장전 시퀀스가 종료되었거나 초기화 대기 중입니다. 추가 R키 입력 무시.");
        //        break;
        //}
    }


    //IEnumerator ReloadStage1Sequence()
    //{
    //    currentReloadStage = ReloadStage.Stage1_Raising; // 1단계 애니메이션 재생 중 상태
    //    isReloadingInputBlocked = true; // 장전 시작했으니 입력 블록

    //    // 1-1) RaiseSword 애니메이션 시작 (Animator Trigger 사용)
    //    anim.SetTrigger("RaiseSword");

    //    // 1-2) RaiseSword 애니메이션 길이만큼 대기
    //    //yield return new WaitForSeconds(reloadRaiseDuration); // 인스펙터에 설정된 길이 사용

    //    // 1-3) 1단계 리듬 체크 (첫 R키 누른 시점의 박자 판정)
    //    Debug.Log(2);
    //    if (0 == gameManager.RhythmCheck()) // 리듬 실패
    //    {
    //        StartCoroutine(ReloadFailSequence());
    //        yield break; // 코루틴 종료
    //    }

    //    currentReloadStage = ReloadStage.Stage1_Complete; // 1단계 애니메이션 완료, 2단계 입력 대기 상태
    //    awaitingRhythmInput = true; // 두 번째 R키 입력 대기 시작

    //    // 1-4) 2단계 리듬 입력 대기 타임아웃
    //    float startTime = Time.time;
    //    while (awaitingRhythmInput && Time.time < startTime + stage1InputTimeout)
    //    {
    //        yield return null; // 다음 프레임까지 대기
    //    }
    //    Debug.Log(3);
    //    // 타임아웃으로 인해 awaitingRhythmInput이 아직 true라면 -> 시간 초과로 실패 처리
    //    if (awaitingRhythmInput)
    //    {
    //        awaitingRhythmInput = false; // 대기 상태 해제
    //        StartCoroutine(ReloadFailSequence());
    //        // isReloadingInputBlocked는 ReloadFailSequence에서 처리됨
    //    }
    //}

    //IEnumerator ReloadStage2Sequence()
    //{
    //    Debug.Log(4);
    //    // currentReloadStage가 Stage1_Complete가 아닌 상태에서 호출될 경우를 대비한 방어 로직
    //    if (currentReloadStage != ReloadStage.Stage1_Complete && currentReloadStage != ReloadStage.Stage2_Ready)
    //    {
    //        StartCoroutine(ReloadFailSequence()); // 즉시 실패 처리
    //        yield break;
    //    }

    //    currentReloadStage = ReloadStage.Stage2_Ready; // 2단계 애니메이션 재생 중 상태
    //    Debug.Log(5);
    //    // 2-1) 리듬 체크 (두 번째 R키 누른 시점의 박자 판정)
    //    int timing2 = GameManager.Instance?.RhythmCheck() ?? 1;

    //    if (timing2 == 0) // 리듬 실패
    //    {
    //        StartCoroutine(ReloadFailSequence());
    //        yield break; // 코루틴 종료
    //    }

    //    Debug.Log(6);
    //    // 2-2) LowerSword 애니메이션 시작 (Animator Trigger 사용)
    //    anim.SetTrigger("LowerSword"); // Stage 2 진입 애니메이션 트리거

    //    // Stage1SuccessTrigger를 발동하여 Animator에서 Stage2로 전환
    //    anim.SetTrigger("Stage1SuccessTrigger"); // 이 줄은 주석이 제거된 상태여야 합니다!

    //    //Debug.Log("장전 2단계 시작: LowerSword 애니메이션 (Original log)");

    //    PlaySparkleOnce(); // Reload 2단계에서만 파티클 재생
    //    yield return new WaitForSeconds(sparkleDuration); // 파티클 유지 시간
    //    Debug.Log(7);
    //    // 2-3) LowerSword 애니메이션 길이만큼 대기
    //    yield return new WaitForSeconds(reloadLowerDuration); // 인스펙터에 설정된 길이 사용
    //    StopSparkle(); // 파티클 정지

    //    // 2-4) 내구도 복구 및 성공 처리
    //    nowAmmo = maxAmmo; // maxAmmo 대신 reloadShot 값을 사용
    //    //Debug.Log($"Reload Complete! Durability: {nowAmmo}");
    //    EventManager.Instance.PlayerCurrentBulletUIRefresh(nowAmmo);
    //    Debug.Log(8);
    //    StartCoroutine(ReloadSuccessSequence()); // 성공 마무리 코루틴 시작
    //}

    //IEnumerator ReloadSuccessSequence()
    //{
    //    currentReloadStage = ReloadStage.Success;
    //    // Animator: "ChargeSuccess" (새로운 Trigger)를 받아서 "Charge Success" 스테이트로 이동하도록 설정
    //    // anim.SetTrigger("ChargeSuccess"); 
    //    //Debug.Log("장전 성공 애니메이션 시퀀스 시작.");

    //    // 성공 애니메이션 재생 시간 대기 (예: 0.5초)
    //    // yield return new WaitForSeconds(0.5f); 
    //    // 만약 특정 성공 애니메이션이 있다면 그 애니메이션이 끝날 때까지 대기

    //    // 상태 초기화
    //    currentReloadStage = ReloadStage.None;
    //    isReloadingInputBlocked = false; // 입력 블록 해제
    //    awaitingRhythmInput = false; // 혹시 모를 대기 상태 해제

    //    // Animator: 성공 후 Idle로 돌아가기 위한 Trigger (예: ResetCharge)
    //    // anim.SetTrigger("ResetCharge"); 

    //    yield break;
    //}


    //IEnumerator ReloadFailSequence()
    //{
    //    // 이미 실패 시퀀스 중이거나 다른 중요한 시퀀스 중이라면 중복 호출 방지
    //    if (currentReloadStage == ReloadStage.Fail)
    //    {
    //        yield break;
    //    }

    //    currentReloadStage = ReloadStage.Fail;
    //    // Animator: "ReloadFail" (새로운 Trigger)을 받아서 "Charge Fail" 스테이트로 이동하도록 설정
    //    anim.SetTrigger("ReloadFail");

    //    // 실패 애니메이션 재생 시간 대기 (예: 1초)
    //    yield return new WaitForSeconds(1.0f); // 실패 애니메이션 길이에 맞게 설정

    //    // 상태 초기화
    //    currentReloadStage = ReloadStage.None;
    //    isReloadingInputBlocked = false; // 입력 블록 해제
    //    awaitingRhythmInput = false; // 혹시 모를 대기 상태 해제

    //    // Animator: 실패 후 Idle로 돌아가기 위한 Trigger (예: ResetCharge)
    //    // anim.SetTrigger("ResetCharge");

    //    yield break;
    //}


    public void WeaponAttack()
    {
        if (nowAmmo <= 0)
        {
            SoundManager.Instance.PlaySFX(SFX.PistolEmpty);
            return;
        }


        // 리로드 중일 때는 공격을 막습니다.
        if (currentReloadStage != ReloadStage.None)
        {
            return;
        }

        // 공격 애니메이션 속도 적용
        //anim.speed = attackAnimationSpeed;

        // WeaponAttack Trigger 발동 (Animator에서 WeaponAttack을 Trigger 타입으로 변경했어야 합니다.)
        anim.SetTrigger("WeaponAttack");

        // SoundManager를 통해 공격 사운드 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SFX.SwordSlash); // SFX.SwordSlash 항목 재생
        }

        nowAmmo -= 1;

        EventManager.Instance.PlayerCurrentBulletUIRefresh(nowAmmo);

        if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
        {
            gameManager.AddCombo();
        }
        else
        {
            Debug.Log("박자 타이밍 실패...");
            SoundManager.Instance.PlaySFX(SFX.RhythmFailShot);
            gameManager.SetHalfCombo();
        }

        Hit();

        gameManager.NotePush();

        // EndAttack 코루틴 시작 (애니메이션 재생 시간에 맞춰 다른 로직 처리용)
        //StartCoroutine(EndAttack());
    }

    void Hit()
    {
        RaycastHit[] results = new RaycastHit[1]; // 미리 배열 생성
        int hitCount = Physics.SphereCastNonAlloc(cameraTr.position, 1f, cameraTr.forward, results, range, LayerMask.GetMask("Enemy"));

        // 적 기본
        if (0 < hitCount)
        {
            Instantiate(hitEffectPrefab, results[0].point, Quaternion.LookRotation(results[0].normal) * Quaternion.Euler(90, 0, 0));

            if (results[0].transform.TryGetComponent(out Entity enemy))
            {
                enemy.GetDamage(GetTotalDamage());
            }

            return;
        }
    }

    //IEnumerator EndAttack()
    //{
    //float waitTime = attackDuration / attackAnimationSpeed;
    //yield return new WaitForSeconds(waitTime);

    //anim.speed = 1f; // 애니메이터 재생 속도를 원래대로 되돌립니다.
    //}

    //void PlaySparkleOnce()
    //{
    //    if (sparkleInstances == null || sparkleInstances.Count == 0)
    //    {
    //    }

    //    foreach (var inst in sparkleInstances)
    //    {
    //        if (inst != null)
    //        {
    //            inst.Clear();
    //            inst.Play();
    //        }
    //        else
    //        {
    //        }
    //    }
    //}

    void StopSparkle()
    {
        foreach (var inst in sparkleInstances)
        {
            if (inst != null)
            {
                inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                inst.Clear();
            }
        }
    }
}