using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sword weapon script that handles swinging, reloading, and multiple sparkle effects.
/// </summary>
public class Sword : WeaponBase
{
    [Header("Sword Settings")]
    [SerializeField] protected int initialDurability = 100;
    [SerializeField] protected int durabilityPerSwing = 1;
    [SerializeField] protected float swingCooldown = 0.5f;
    [SerializeField] protected float reloadRaiseDuration = 0.5f; // 검 들어올리는 애니메이션 길이
    [SerializeField] protected float sparkleDuration = 0.3f; // 파티클 유지 시간
    [SerializeField] protected float reloadLowerDuration = 0.5f; // 검 내리는 애니메이션 길이
    [SerializeField] protected int reloadShot = 100;

    [Header("Particle Settings")]
    [Tooltip("Particle System Prefabs for sparkle effect")]
    [SerializeField] protected List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();
    [SerializeField] protected int sparkleCount = 30; // Emit 개수 per system

    [Header("References")]
    [SerializeField] protected Animator swordAnimator;

    private bool isReloading = false;

    // 외부에서 사용할 수 있는 프로퍼티
    public float ReloadRaiseDuration => reloadRaiseDuration;
    public float SparkleDuration => sparkleDuration;

    private void Awake()
    {
        // WeaponBase 초기화
        maxAmmo = initialDurability;
        nowAmmo = initialDurability;
        shotAmount = durabilityPerSwing;
        nextShotTime = 0f;

        // Sparkle Prefabs 인스턴스화하여 자식으로 붙이고 초기화
        sparkleInstances.Clear();
        if (sparklePrefabs != null)
        {
            foreach (var prefab in sparklePrefabs)
            {
                if (prefab != null)
                {
                    var inst = Instantiate(prefab, transform);
                    inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    sparkleInstances.Add(inst);
                }
            }
        }
    }

    /// <summary> 칼 휘두르기 (내구도 차감 + 애니) </summary>
    public void Swing()
    {
        if (isReloading || Time.time < nextShotTime || nowAmmo < shotAmount)
            return;

        nowAmmo -= shotAmount;
        nextShotTime = Time.time + swingCooldown;
        swordAnimator?.SetTrigger("Swing");
        Debug.Log($"Swing! Durability: {nowAmmo}");
    }

    /// <summary> WeaponBase.Shoot() 호출 시 Swing 실행 </summary>
    protected override void Shoot() => Swing();

    /// <summary> WeaponBase.Reload() 호출 시 ReloadRoutine 실행 </summary>
    protected override void Reload()
    {
        if (isReloading || nowAmmo >= maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // 1) 검 들어올리기 애니
        swordAnimator?.SetTrigger("RaiseSword");
        yield return new WaitForSeconds(reloadRaiseDuration);

        // 2) 파티클 Emit 방식으로 한 번만 방출
        foreach (var inst in sparkleInstances)
        {
            inst.Clear();
            inst.Emit(sparkleCount);
        }

        // 3) sparkleDuration 만큼 대기
        yield return new WaitForSeconds(sparkleDuration);

        // 4) 파티클 정리
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Clear();
        }

        // 5) 검 내리기 애니
        swordAnimator?.SetTrigger("LowerSword");
        yield return new WaitForSeconds(reloadLowerDuration);

        // 6) 내구도 복구 & 루틴 종료
        nowAmmo = Mathf.Clamp(reloadShot, 0, maxAmmo);
        isReloading = false;
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");
    }

    /// <summary> 한 번만 Sparkle 효과 재생 (Draw 시 호출) </summary>
    public void PlaySparkleOnce()
    {
        // Play() 를 사용하여 Burst 모듈 설정대로 방출
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Play();
        }
        // sparkleDuration 후 정리
        Invoke(nameof(StopSparkle), sparkleDuration);
    }

    /// <summary> 즉시 Sparkle 효과 정리 </summary>
    public void StopSparkle()
    {
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Clear();
        }
    }
}
