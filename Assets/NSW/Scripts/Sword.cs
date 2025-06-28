using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using UnityEngine.Windows; // 이 네임스페이스는 일반적으로 불필요하며 오류의 원인이 될 수 있어 제거했습니다.

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
    [Tooltip("공격 애니메이션 속도")]
    [SerializeField] float attackAnimationSpeed = 1f;
    [Tooltip("공격 애니메이션 실제 길이 (초)")]
    // 이 값은 Animation 창에서 'Sword Slash' 애니메이션의 실제 길이를 확인하여 정확하게 설정해야 합니다.
    // 예: 0.16초
    [SerializeField] float attackDuration = 0.18f; // 이전 대화에서 0.16초 정도로 확인되었으므로 조정
    // [Tooltip("공격 쿨다운 (초)")] // 더 이상 직접적인 쿨다운으로 사용되지 않습니다.
    [SerializeField] float attackCooldown = 0.8f; // 이 변수 자체는 남아있지만 코드에서는 사용되지 않습니다.

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Reload Settings")]
    [Tooltip("재장전 후 회복될 내구도(탄환)량")]
    [SerializeField] int reloadShot = 100; // 누락되었던 reloadShot 변수 재추가
    [Tooltip("1단계 장전 후 다음 R키 입력까지 대기 시간 (타임아웃)")]
    [SerializeField] float stage1InputTimeout = 2.0f;

    // bool isDrawing = false; // DrawSwordSequence 코루틴이 주석 처리되었으므로 이 플래그도 사용되지 않습니다.
    // bool isAttacking = false; // WeaponAttack이 Trigger 방식으로 변경되면서 이 플래그는 더 이상 필요 없습니다.
    // float nextAttackTime = 0f; // 공격 쿨다운 로직 제거로 이 변수도 더 이상 사용되지 않습니다.

    // 장전 중 중복 입력 방지를 위한 플래그
    bool isReloadingInputBlocked = false;
    // 리듬 입력 대기 중인지 여부 (정확한 타이밍에 R키를 눌러야 할 때 사용)
    bool awaitingRhythmInput = false;

    // anim 변수는 WeaponBase에서 참조하고 있다고 가정합니다.
    // [SerializeField] Animator swordAnimator; // 이 줄은 제거되었습니다.

    protected override void Awake()
    {
        base.Awake(); // WeaponBase의 Awake 호출

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

        // 0번 키로 Draw (DrawSwordSequence 코루틴이 주석 처리되어 현재 작동하지 않습니다.)
        // KeyCode drawKey = KeyCode.Alpha0 + weaponNum;
        // if (!isDrawing && Input.GetKeyDown(drawKey) && currentReloadStage == ReloadStage.None)
        // {
        //     StartCoroutine(DrawSwordSequence());
        // }

        // R 키로 Reload (장전 입력 블록 중이 아닐 때만 허용)
        if (!isReloadingInputBlocked && input.r_Input)
        {
            input.r_Input = false; // 입력 소비

            HandleReloadInput();
        }

        // 좌클릭으로 Attack
        // 이제 매번 클릭 시도 (쿨다운, 내구도, isAttacking 체크 없이)
        if (input.mouse0_Input)
        {
            input.mouse0_Input = false; // 입력 소비

            WeaponAttack();
        }
    }

    /// <summary>
    /// R키 입력을 현재 장전 상태에 따라 처리합니다.
    /// </summary>
    void HandleReloadInput()
    {
        //Debug.Log($"[ReloadInput] R key pressed. currentReloadStage: {currentReloadStage}, awaitingRhythmInput: {awaitingRhythmInput}");
        switch (currentReloadStage)
        {
            case ReloadStage.None: // 초기 상태: 첫 R키 입력 -> 1단계 장전 시작
                //Debug.Log("[ReloadInput] Initiating Reload Stage 1.");
                StartCoroutine(ReloadStage1Sequence());
                break;

            case ReloadStage.Stage1_Complete: // 1단계 완료: 두 번째 R키 입력 대기 중
                //awaitingRhythmInput이 true일 때만 R키 입력이 유효하도록
                if (awaitingRhythmInput)
                {
                    Debug.Log("[ReloadInput] Detected 2nd R key for Stage 2 transition.");
                    awaitingRhythmInput = false; // R키 입력 받았으니 대기 상태 해제
                    StartCoroutine(ReloadStage2Sequence());
                }
                else
                {
                    Debug.Log("[ReloadInput] 2nd R key pressed at wrong timing (timeout or already processed).");
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

    // IEnumerator DrawSwordSequence() 코루틴 전체가 주석 처리되어 있습니다.
    // 만약 발도 애니메이션을 다시 활성화하려면 주석을 해제하고 weaponNum 변수를 처리해야 합니다.

    /// <summary>
    /// 1단계 장전 시퀀스: RaiseSword 애니메이션 재생 및 첫 번째 리듬 체크
    /// </summary>
    IEnumerator ReloadStage1Sequence()
    {
        //Debug.Log("[ReloadStage1] Entering ReloadStage1Sequence.");
        currentReloadStage = ReloadStage.Stage1_Raising; // 1단계 애니메이션 재생 중 상태
        isReloadingInputBlocked = true; // 장전 시작했으니 입력 블록

        // 1-1) RaiseSword 애니메이션 시작 (Animator Trigger 사용)
        anim.SetTrigger("RaiseSword");
        //Debug.Log("[ReloadStage1] Starting RaiseSword animation. Trigger 'RaiseSword' sent.");

        // 1-2) RaiseSword 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(reloadRaiseDuration); // 인스펙터에 설정된 길이 사용

        // 1-3) 1단계 리듬 체크 (첫 R키 누른 시점의 박자 판정)
        int timing1 = GameManager.Instance?.RhythmCheck() ?? 1; // GameManager.Instance가 없으면 성공(1)으로 처리
        //Debug.Log($"[ReloadStage1] RhythmCheck result: {timing1}.");

        if (timing1 == 0) // 리듬 실패
        {
            Debug.Log("[ReloadStage1] Rhythm failed. Initiating Reload Fail Sequence.");
            StartCoroutine(ReloadFailSequence());
            yield break; // 코루틴 종료
        }
        //Debug.Log("[ReloadStage1] Rhythm success. Setting to Stage1_Complete, awaiting 2nd R key.");

        currentReloadStage = ReloadStage.Stage1_Complete; // 1단계 애니메이션 완료, 2단계 입력 대기 상태
        awaitingRhythmInput = true; // 두 번째 R키 입력 대기 시작

        // 1-4) 2단계 리듬 입력 대기 타임아웃
        float startTime = Time.time;
        while (awaitingRhythmInput && Time.time < startTime + stage1InputTimeout)
        {
            yield return null; // 다음 프레임까지 대기
        }

        // 타임아웃으로 인해 awaitingRhythmInput이 아직 true라면 -> 시간 초과로 실패 처리
        if (awaitingRhythmInput)
        {
            Debug.Log("[ReloadStage1] Timeout for 2nd R key. Initiating Reload Fail Sequence.");
            awaitingRhythmInput = false; // 대기 상태 해제
            StartCoroutine(ReloadFailSequence());
            // isReloadingInputBlocked는 ReloadFailSequence에서 처리됨
        }
    }

    /// <summary>
    /// 2단계 장전 시퀀스: LowerSword 애니메이션 재생 및 두 번째 리듬 체크
    /// </summary>
    IEnumerator ReloadStage2Sequence()
    {
        Debug.Log($"[ReloadStage2] Entering ReloadStage2Sequence. currentReloadStage: {currentReloadStage}");
        // currentReloadStage가 Stage1_Complete가 아닌 상태에서 호출될 경우를 대비한 방어 로직
        if (currentReloadStage != ReloadStage.Stage1_Complete && currentReloadStage != ReloadStage.Stage2_Ready)
        {
            Debug.LogWarning("[ReloadStage2] Invalid stage entry. Initiating Reload Fail Sequence.");
            StartCoroutine(ReloadFailSequence()); // 즉시 실패 처리
            yield break;
        }

        currentReloadStage = ReloadStage.Stage2_Ready; // 2단계 애니메이션 재생 중 상태

        // 2-1) 리듬 체크 (두 번째 R키 누른 시점의 박자 판정)
        int timing2 = GameManager.Instance?.RhythmCheck() ?? 1;
        Debug.Log($"[ReloadStage2] 2nd RhythmCheck result: {timing2}.");

        if (timing2 == 0) // 리듬 실패
        {
            Debug.Log("[ReloadStage2] 2nd Rhythm failed. Initiating Reload Fail Sequence.");
            StartCoroutine(ReloadFailSequence());
            yield break; // 코루틴 종료
        }

        Debug.Log($"[ReloadStage2] 2nd Rhythm success. Triggering LowerSword and Stage1SuccessTrigger.");

        // 2-2) LowerSword 애니메이션 시작 (Animator Trigger 사용)
        anim.SetTrigger("LowerSword"); // Stage 2 진입 애니메이션 트리거

        // Stage1SuccessTrigger를 발동하여 Animator에서 Stage2로 전환
        anim.SetTrigger("Stage1SuccessTrigger"); // 이 줄은 주석이 제거된 상태여야 합니다!

        Debug.Log("장전 2단계 시작: LowerSword 애니메이션 (Original log)");

        PlaySparkleOnce(); // Reload 2단계에서만 파티클 재생
        yield return new WaitForSeconds(sparkleDuration); // 파티클 유지 시간

        // 2-3) LowerSword 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(reloadLowerDuration); // 인스펙터에 설정된 길이 사용
        StopSparkle(); // 파티클 정지

        // 2-4) 내구도 복구 및 성공 처리
        nowAmmo = reloadShot; // maxAmmo 대신 reloadShot 값을 사용
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");

        StartCoroutine(ReloadSuccessSequence()); // 성공 마무리 코루틴 시작
    }

    /// <summary>
    /// 장전 성공 시퀀스: 성공 애니메이션 재생 및 상태 초기화
    /// </summary>
    IEnumerator ReloadSuccessSequence()
    {
        Debug.Log("[ReloadSuccess] Entering ReloadSuccessSequence.");
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

        Debug.Log("[ReloadSuccess] Reload success sequence complete and reset.");
        yield break;
    }

    /// <summary>
    /// 장전 실패 시퀀스: 실패 애니메이션 재생 및 상태 초기화
    /// </summary>
    IEnumerator ReloadFailSequence()
    {
        Debug.Log("[ReloadFail] Entering ReloadFailSequence.");
        // 이미 실패 시퀀스 중이거나 다른 중요한 시퀀스 중이라면 중복 호출 방지
        if (currentReloadStage == ReloadStage.Fail)
        {
            Debug.Log("[ReloadFail] Already in fail sequence, preventing re-entry.");
            yield break;
        }

        currentReloadStage = ReloadStage.Fail;
        // Animator: "ReloadFail" (새로운 Trigger)을 받아서 "Charge Fail" 스테이트로 이동하도록 설정
        anim.SetTrigger("ReloadFail");
        Debug.Log("장전 실패 애니메이션 시퀀스 시작. Trigger 'ReloadFail' sent.");

        // 실패 애니메이션 재생 시간 대기 (예: 1초)
        yield return new WaitForSeconds(1.0f); // 실패 애니메이션 길이에 맞게 설정

        // 상태 초기화
        currentReloadStage = ReloadStage.None;
        isReloadingInputBlocked = false; // 입력 블록 해제
        awaitingRhythmInput = false; // 혹시 모를 대기 상태 해제

        // Animator: 실패 후 Idle로 돌아가기 위한 Trigger (예: ResetCharge)
        // anim.SetTrigger("ResetCharge"); 

        Debug.Log("[ReloadFail] Reload fail sequence complete and reset. State reset to None, ready for retry.");
        yield break;
    }

    /// <summary>
    /// WeaponAttack: 마우스 좌클릭 시마다 Sword Slash 애니메이션을 재생합니다.
    /// (내구도, 쿨다운, isAttacking 플래그 체크 없이)
    /// </summary>
    public void WeaponAttack()
    {
        // 디버그 로그 추가
        //Debug.Log($"[Attack] Attempting attack. currentReloadStage: {currentReloadStage}");

        // 리로드 중일 때는 공격을 막습니다.
        if (currentReloadStage != ReloadStage.None)
        {
            Debug.Log("[Attack] Attack blocked: Currently reloading.");
            return;
        }

        // 모든 쿨다운, 내구도, isAttacking 플래그 체크를 제거합니다.
        // 이제 공격 쿨다운, 내구도 소모는 스크립트 상에서 직접 처리되지 않습니다.
        // 애니메이션은 클릭 시마다 즉시 발동됩니다.

        // 공격 애니메이션 속도 적용
        anim.speed = attackAnimationSpeed;

        // WeaponAttack Trigger 발동 (Animator에서 WeaponAttack을 Trigger 타입으로 변경했어야 합니다.)
        anim.SetTrigger("WeaponAttack");
        //Debug.Log("[Attack] WeaponAttack Trigger sent.");

        // EndAttack 코루틴 시작 (애니메이션 재생 시간에 맞춰 다른 로직 처리용)
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        float waitTime = attackDuration / attackAnimationSpeed;
        //Debug.Log($"[EndAttack] Starting EndAttack coroutine. Waiting for: {waitTime} seconds.");
        yield return new WaitForSeconds(waitTime);

        //Debug.Log("[EndAttack] Wait finished. Animator speed reset.");
        anim.speed = 1f; // 애니메이터 재생 속도를 원래대로 되돌립니다.
    }

    void PlaySparkleOnce()
    {
        Debug.Log("[Particle] PlaySparkleOnce called.");
        if (sparkleInstances == null || sparkleInstances.Count == 0)
        {
            Debug.LogWarning("[Particle] sparkleInstances is null or empty. No particles to play.");
        }

        foreach (var inst in sparkleInstances)
        {
            if (inst != null)
            {
                inst.Clear();
                inst.Play();
                Debug.Log($"[Particle] Playing particle system: {inst.name}");
            }
            else
            {
                Debug.LogWarning("[Particle] Null particle system found in sparkleInstances.");
            }
        }
    }

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

    // WeaponBase 의 기본 Shoot/Reload 는 사용하지 않음
    protected override void Shoot() { }
    protected override void Reload() { } // 이 Reload 함수는 사용하지 않으므로 비워둠
}