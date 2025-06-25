using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Animator Settings")]
    [SerializeField] Animator swordAnimator;   // 반드시 Inspector에서 할당!

    bool isDrawing = false;
    bool isReloading = false;
    bool isAttacking = false;
    float nextAttackTime = 0f;

    void Awake()
    {
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
        }

        // 좌클릭으로 Attack
        if (!isAttacking && Time.time >= nextAttackTime && Input.GetMouseButtonDown(0))
        {
            WeaponAttack();
        }
    }

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

    IEnumerator ReloadSequence()
    {
        isReloading = true;

        // 1) 리듬 체크
        int timing = GameManager.Instance?.RhythmCheck() ?? 1;
        if (timing == 0)
        {
            Debug.Log("리듬 실패: Reload 단계");
            isReloading = false;
            yield break;
        }
        Debug.Log($"리듬 성공({timing}): Reload 단계");

        // 2) RaiseSword 애니 + 파티클
        swordAnimator.SetBool("WeaponReload_0", true);
        swordAnimator.SetTrigger("RaiseSword");
        yield return new WaitForSeconds(reloadRaiseDuration);

        PlaySparkleOnce();
        yield return new WaitForSeconds(sparkleDuration);

        // 3) LowerSword 애니
        swordAnimator.SetBool("WeaponReload_0", false);
        swordAnimator.SetTrigger("LowerSword");
        yield return new WaitForSeconds(reloadLowerDuration);

        StopSparkle();

        // 4) 내구도 복구
        nowAmmo = Mathf.Clamp(reloadShot, 0, maxAmmo);
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");

        isReloading = false;
    }

    /// <summary>
    /// WeaponAttack: 
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
        nextAttackTime = Time.time + attackCooldown;

        // 공격 애니 시작
        isAttacking = true;
        swordAnimator.speed = attackAnimationSpeed;
        swordAnimator.SetBool("WeaponAttack", true);

        // 애니 길이 뒤에 종료 처리
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackDuration / attackAnimationSpeed);
        swordAnimator.SetBool("WeaponAttack", false);
        swordAnimator.speed = 1f;
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
