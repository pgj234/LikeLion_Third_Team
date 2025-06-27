using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows;

public class Sword : WeaponBase
{
    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;   // Draw Sword 2 애니 길이
    [SerializeField] float sparkleDuration = 0.3f;   // 파티클 유지 시간
    [SerializeField] float reloadLowerDuration = 0.5f;   // Put 애니 길이

    [Header("Attack Settings")]
    [Tooltip("공격 애니메이션 속도")]
    [SerializeField] float attackAnimationSpeed = 1f;
    [Tooltip("공격 애니메이션 실제 길이 (초)")]
    [SerializeField] float attackDuration = 0.6f;
    [Tooltip("공격 쿨다운 (초)")]
    [SerializeField] float attackCooldown = 0.8f;

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Reload Settings")]
    [Tooltip("재장전 후 회복될 내구도(탄환)량")]
    [SerializeField] int reloadShot = 100;

<<<<<<< HEAD
    [SerializeField] Avatar avatar;
=======
    [Header("Animator Settings")]
    [SerializeField] Animator swordAnimator;   // 반드시 Inspector에서 할당!
>>>>>>> parent of 2a1ff71 (NSW)

    bool isDrawing = false;
    bool isReloading = false;
    bool isAttacking = false;
    float nextAttackTime = 0f;

<<<<<<< HEAD
    // 장전 중 중복 입력 방지를 위한 플래그
    bool isReloadingInputBlocked = false;
    // 리듬 입력 대기 중인지 여부 (정확한 타이밍에 R키를 눌러야 할 때 사용)
    bool awaitingRhythmInput = false;

    protected override void Awake()
=======
    void Awake()
>>>>>>> parent of 2a1ff71 (NSW)
    {
        base.Awake();

        // Sparkle 프리팹 인스턴스 생성 및 초기화
        foreach (var prefab in sparklePrefabs)
        {
            if (prefab == null) continue;
            var inst = Instantiate(prefab, transform);
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sparkleInstances.Add(inst);
        }
    }

    protected override void Update()
    {
        base.Update();

<<<<<<< HEAD
        // R 키로 Reload (장전 입력 블록 중이 아닐 때만 허용)
        if (!isReloadingInputBlocked && input.r_Input)
        {
            input.r_Input = false;

            HandleReloadInput();
=======
        // 0번 키로 Draw
        KeyCode drawKey = KeyCode.Alpha0 + weaponNum;
        if (!isDrawing && Input.GetKeyDown(drawKey))
        {
            StartCoroutine(DrawSwordSequence());
        }

        // R 키로 Reload
        if (!isReloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadSequence());
>>>>>>> parent of 2a1ff71 (NSW)
        }

        // 좌클릭으로 Attack
        if (!isAttacking && Time.time >= nextAttackTime && input.mouse0_Input)
        {
            input.mouse0_Input = false;

            WeaponAttack();
        }
    }

<<<<<<< HEAD
    void OnEnable()
    {
        anim.avatar = avatar;
        anim.enabled = true;
    }

    void OnDisable()
    {
        anim.avatar = null;
        anim.enabled = false;
    }

    /// <summary>
    /// R키 입력을 현재 장전 상태에 따라 처리합니다.
    /// </summary>
    void HandleReloadInput()
    {
        switch (currentReloadStage)
        {
            case ReloadStage.None: // 초기 상태: 첫 R키 입력 -> 1단계 장전 시작
                StartCoroutine(ReloadStage1Sequence());
                break;

            case ReloadStage.Stage1_Complete: // 1단계 완료: 두 번째 R키 입력 대기 중
                //awaitingRhythmInput이 true일 때만 R키 입력이 유효하도록
                if (awaitingRhythmInput)
                {
                    awaitingRhythmInput = false; // R키 입력 받았으니 대기 상태 해제
                    StartCoroutine(ReloadStage2Sequence());
                }
                else
                {
                    Debug.Log("장전 2단계 입력 타이밍이 아닙니다. (이미 지났거나 아직 오지 않음)");
                    // 이미 타임아웃 되었거나, 아직 1단계 애니메이션 중일 수 있음.
                    // 이 경우, 플레이어에게 피드백을 주거나,
                    // 무효한 입력으로 간주하고 현재 장전 상태를 실패로 돌릴 수도 있습니다.
                    StartCoroutine(ReloadFailSequence()); // 즉시 실패 처리하는 경우
                }
                break;

            case ReloadStage.Stage1_Raising: // 1단계 애니메이션이 재생 중일 때 R키 입력
                Debug.Log("장전 1단계 애니메이션 중입니다. 잠시 기다려주세요.");
                // 이 상황에서 R키를 다시 누르면 씹히거나, 처음부터 다시 시작하게 할 수도 있습니다.
                break;

            case ReloadStage.Success: // 장전 성공 후: 추가 R키 입력 무시
            case ReloadStage.Fail: // 장전 실패 후: 추가 R키 입력 무시
                Debug.Log("장전 시퀀스가 종료되었거나 초기화 대기 중입니다. 추가 R키 입력 무시.");
                break;
        }
    }

    //IEnumerator DrawSwordSequence()
    //{
    //    isDrawing = true;
    //    isReloadingInputBlocked = true; // 그리는 동안은 장전 입력도 블록

    //    // Draw 모션 시작 (Animator 트랜지션 사용)
    //    anim.SetBool("WeaponPull", true);

    //    // --- 이 부분에 디버그 로그 추가 ---
    //    swordDrawSound = GetComponent<SwordDrawSound>();
    //    if (swordDrawSound == null)
    //    {
    //        Debug.LogWarning("Sword.cs: SwordDrawSound 컴포넌트를 찾을 수 없습니다! 동일 GameObject에 있나요?");
    //    }
    //    else
    //    {
    //        Debug.Log("Sword.cs: SwordDrawSound 컴포넌트를 성공적으로 찾았습니다.");
    //    }
    //    // ----------------------------------

    //    PlaySparkleOnce();

    //    // 애니메이션 길이 대기 (Animator의 특정 State의 길이를 사용하는 것이 더 정확합니다)
    //    // 여기서는 reloadRaiseDuration을 Draw Sword 2 애니메이션 길이로 사용한다고 가정
    //    yield return new WaitForSeconds(reloadRaiseDuration);

    //    // 파티클 정지 & Idle 복귀 (Animator 트랜지션 사용)
    //    StopSparkle();
    //    anim.SetBool("WeaponPull", false); // WeaponPull이 false가 되면 Idle로 트랜지션되도록 Animator 설정

    //    isDrawing = false;
    //    isReloadingInputBlocked = false; // 드로잉 끝났으니 장전 입력 블록 해제
    //}
=======
    IEnumerator DrawSwordSequence()
    {
        isDrawing = true;

        // Draw 모션 + 파티클
        swordAnimator.SetBool("WeaponPull", true);
        swordAnimator.Play("Draw Sword 2", 0, 0f);
        PlaySparkleOnce();

        // Draw 애니 길이 대기
        yield return new WaitForSeconds(reloadRaiseDuration);

        // 파티클 정지 & Idle 복귀
        StopSparkle();
        swordAnimator.SetBool("WeaponPull", false);
        swordAnimator.Play("Idle Walk Run Blend", 0, 0f);

        isDrawing = false;
    }
>>>>>>> parent of 2a1ff71 (NSW)

    IEnumerator ReloadSequence()
    {
        isReloading = true;

<<<<<<< HEAD
        // 1-1) RaiseSword 애니메이션 시작 (Animator Trigger 사용)
        // Animator: "RaiseSword" Trigger를 받아서 "Raise Sword" 스테이트로 이동하도록 설정
        anim.SetTrigger("RaiseSword");
        Debug.Log("장전 1단계 시작: RaiseSword 애니메이션");

        // 1-2) RaiseSword 애니메이션 길이만큼 대기
        // 실제 애니메이션 클립의 길이를 여기에 사용하거나, Animation Event로 제어하는 것이 이상적
        yield return new WaitForSeconds(reloadRaiseDuration); // 인스펙터에 설정된 길이 사용

        // 1-3) 1단계 리듬 체크 (첫 R키 누른 시점의 박자 판정)
        int timing1 = GameManager.Instance?.RhythmCheck() ?? 1; // GameManager.Instance가 없으면 성공(1)으로 처리
        if (timing1 == 0) // 리듬 실패
        {
            Debug.Log("리듬 실패: 1단계 장전 박자 불일치");
            StartCoroutine(ReloadFailSequence());
            yield break; // 코루틴 종료
        }
        Debug.Log($"리듬 성공({timing1}): 1단계 장전 박자 일치. 2단계 입력 대기.");

        currentReloadStage = ReloadStage.Stage1_Complete; // 1단계 애니메이션 완료, 2단계 입력 대기 상태
        awaitingRhythmInput = true; // 두 번째 R키 입력 대기 시작

        // 1-4) 2단계 리듬 입력 대기 타임아웃
        //float startTime = Time.time;
        //while (awaitingRhythmInput && Time.time < startTime + stage1InputTimeout)
        //{
        //    yield return null; // 다음 프레임까지 대기
        //}

        // 타임아웃으로 인해 awaitingRhythmInput이 아직 true라면 -> 시간 초과로 실패 처리
        //if (awaitingRhythmInput)
        //{
        //    Debug.Log("리듬 입력 시간 초과 (2단계 입력 대기 중)");
        //    awaitingRhythmInput = false; // 대기 상태 해제
        //    StartCoroutine(ReloadFailSequence());
        //    // isReloadingInputBlocked는 ReloadFailSequence에서 처리됨
        //}
        // 만약 R키가 눌렸다면 ReloadStage2Sequence가 호출되어 이 코루틴이 먼저 종료됨.
    }

    /// <summary>
    /// 2단계 장전 시퀀스: LowerSword 애니메이션 재생 및 두 번째 리듬 체크
    /// </summary>
    IEnumerator ReloadStage2Sequence()
    {
        // currentReloadStage가 Stage1_Complete가 아닌 상태에서 호출될 경우를 대비한 방어 로직
        if (currentReloadStage != ReloadStage.Stage1_Complete && currentReloadStage != ReloadStage.Stage2_Ready)
        {
            Debug.LogWarning("잘못된 장전 단계에서 ReloadStage2Sequence 호출 시도. (현재 상태: " + currentReloadStage + ")");
            //StartCoroutine(ReloadFailSequence()); // 즉시 실패 처리
=======
        // 1) 리듬 체크
        int timing = GameManager.Instance?.RhythmCheck() ?? 1;
        if (timing == 0)
        {
            Debug.Log("리듬 실패: Reload 단계");
            isReloading = false;
>>>>>>> parent of 2a1ff71 (NSW)
            yield break;
        }
        Debug.Log($"리듬 성공({timing}): Reload 단계");

        // 2) RaiseSword 애니 + 파티클
        swordAnimator.SetBool("WeaponReload_0", true);
        swordAnimator.SetTrigger("RaiseSword");
        yield return new WaitForSeconds(reloadRaiseDuration);

<<<<<<< HEAD
        // 2-1) 리듬 체크 (두 번째 R키 누른 시점의 박자 판정)
        int timing2 = GameManager.Instance?.RhythmCheck() ?? 1;
        if (timing2 == 0) // 리듬 실패
        {
            Debug.Log("리듬 실패: 2단계 장전 박자 불일치");
            //StartCoroutine(ReloadFailSequence());
            yield break; // 코루틴 종료
        }

        Debug.Log($"리듬 성공({timing2}): 2단계 장전 박자 일치.");

        // 2-2) LowerSword 애니메이션 시작 (Animator Trigger 사용)
        // Animator: "LowerSword" Trigger를 받아서 "Lower Sword" 스테이트 또는 "Charge Success" 스테이트로 이동하도록 설정
        anim.SetTrigger("LowerSword");
        Debug.Log("장전 2단계 시작: LowerSword 애니메이션");
=======
        PlaySparkleOnce();
        yield return new WaitForSeconds(sparkleDuration);

        // 3) LowerSword 애니
        swordAnimator.SetBool("WeaponReload_0", false);
        swordAnimator.SetTrigger("LowerSword");
        yield return new WaitForSeconds(reloadLowerDuration);
>>>>>>> parent of 2a1ff71 (NSW)

        StopSparkle();

<<<<<<< HEAD
        // 2-3) LowerSword 애니메이션 길이만큼 대기
        //yield return new WaitForSeconds(reloadLowerDuration); // 인스펙터에 설정된 길이 사용
        StopSparkle(); // 파티클 정지

        // 2-4) 내구도 복구 및 성공 처리
        nowAmmo = maxAmmo;
=======
        // 4) 내구도 복구
        nowAmmo = Mathf.Clamp(reloadShot, 0, maxAmmo);
>>>>>>> parent of 2a1ff71 (NSW)
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");

        isReloading = false;
    }

    /// <summary>
<<<<<<< HEAD
    /// 장전 성공 시퀀스: 성공 애니메이션 재생 및 상태 초기화
    /// </summary>
    IEnumerator ReloadSuccessSequence()
    {
        currentReloadStage = ReloadStage.Success;
        // Animator: "ChargeSuccess" (새로운 Trigger)를 받아서 "Charge Success" 스테이트로 이동하도록 설정
        // anim.SetTrigger("ChargeSuccess"); 
        Debug.Log("장전 성공 애니메이션 시퀀스 시작.");

        // 성공 애니메이션 재생 시간 대기 (예: 0.5초)
        // yield return new WaitForSeconds(0.5f); 
        // 만약 특정 성공 애니메이션이 있다면 그 애니메이션이 끝날 때까지 대기

        // 상태 초기화
        currentReloadStage = ReloadStage.None;
        isReloadingInputBlocked = false; // 입력 블록 해제
        awaitingRhythmInput = false; // 혹시 모를 대기 상태 해제

        // Animator: 성공 후 Idle로 돌아가기 위한 Trigger (예: ResetCharge)
        // anim.SetTrigger("ResetCharge"); 

        Debug.Log("장전 성공 시퀀스 완료 및 초기화.");
        yield break;
    }

    /// <summary>
    /// 장전 실패 시퀀스: 실패 애니메이션 재생 및 상태 초기화
    /// </summary>
    IEnumerator ReloadFailSequence()
    {
        // 이미 실패 시퀀스 중이거나 다른 중요한 시퀀스 중이라면 중복 호출 방지
        if (currentReloadStage == ReloadStage.Fail)
        {
            yield break;
        }

        currentReloadStage = ReloadStage.Fail;
        // Animator: "ReloadFail" (새로운 Trigger)을 받아서 "Charge Fail" 스테이트로 이동하도록 설정
        anim.SetTrigger("ReloadFail");
        Debug.Log("장전 실패 애니메이션 시퀀스 시작.");

        // 실패 애니메이션 재생 시간 대기 (예: 1초)
        yield return new WaitForSeconds(1.0f); // 실패 애니메이션 길이에 맞게 설정

        // 상태 초기화
        currentReloadStage = ReloadStage.None;
        isReloadingInputBlocked = false; // 입력 블록 해제
        awaitingRhythmInput = false; // 혹시 모를 대기 상태 해제

        // Animator: 실패 후 Idle로 돌아가기 위한 Trigger (예: ResetCharge)
        // anim.SetTrigger("ResetCharge"); 

        Debug.Log("장전 실패 시퀀스 완료 및 초기화.");
        yield break;
    }

    /// <summary>
    /// WeaponAttack:  (기존과 동일)
=======
    /// WeaponAttack: 
>>>>>>> parent of 2a1ff71 (NSW)
    /// - 애니메이션 속도 적용, Bool 파라미터 세팅 
    /// - 내구도(nowAmmo) 감소, 쿨다운 적용
    /// </summary>
    public void WeaponAttack()
    {
        // 쿨다운 & 내구도 체크
        if (Time.time < nextAttackTime || nowAmmo < shotAmount)
            return;

        // 내구도 차감 + 쿨다운 갱신
        nowAmmo -= shotAmount;
        nextAttackTime = Time.time;

        // 공격 애니 시작
        isAttacking = true;
<<<<<<< HEAD
        //anim.speed = attackAnimationSpeed;
        anim.Play("Attack", -1, 0);
        //anim.SetBool("WeaponAttack", true); // Animator에 WeaponAttack bool 파라미터 필요
=======
        swordAnimator.speed = attackAnimationSpeed;
        swordAnimator.SetBool("WeaponAttack", true);
>>>>>>> parent of 2a1ff71 (NSW)

        // 애니 길이 뒤에 종료 처리
        //StartCoroutine(EndAttack());
        EndAttack();
    }

    void EndAttack()
    {
        //yield return new WaitForSeconds(attackDuration / attackAnimationSpeed);
        anim.SetBool("WeaponAttack", false);
        anim.speed = 1f;
        isAttacking = false;
    }

    void PlaySparkleOnce()
    {
        foreach (var inst in sparkleInstances)
        {
            inst.Clear();
            inst.Play();
        }
        Invoke(nameof(StopSparkle), sparkleDuration);
    }

    void StopSparkle()
    {
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Clear();
        }
    }

    // WeaponBase 의 기본 Shoot/Reload 는 사용하지 않음
    protected override void Shoot() { }
    protected override void Reload() { }
}
