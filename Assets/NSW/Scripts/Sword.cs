using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sword : WeaponBase
{
    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;   // Draw Sword 2 애니 길이
    [SerializeField] float sparkleDuration = 0.3f;   // 파티클 유지 시간
    [SerializeField] float reloadLowerDuration = 0.5f;   // Put 애니 길이

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Reload Settings")]
    [Tooltip("재장전 후 회복될 내구도(탄환)량")]
    [SerializeField] int reloadShot = 100;

    [Header("Animator Settings")]
    [SerializeField] Animator swordAnimator;             // Inspector에서 반드시 할당!

    bool isDrawing = false;
    bool isReloading = false;

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

        // WeaponNum (0-9) 입력으로 Draw Sequence
        KeyCode drawKey = KeyCode.Alpha0 + weaponNum;
        if (!isDrawing && Input.GetKeyDown(drawKey))
        {
            StartCoroutine(DrawSwordSequence());
        }

        // R 키 입력으로 Reload Sequence
        if (!isReloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ReloadSequence());
        }
    }

    /// <summary>
    /// “0” 키로만 실행.
    /// Draw 애니 + 파티클 → 바로 Idle 복귀.
    /// (리듬 체크 없음)
    /// </summary>
    IEnumerator DrawSwordSequence()
    {
        isDrawing = true;

        // Draw 모션과 파티클
        swordAnimator.SetBool("WeaponPull", true);
        swordAnimator.Play("Draw Sword 2", 0, 0f);
        PlaySparkleOnce();

        // Draw 애니 길이 대기
        yield return new WaitForSeconds(reloadRaiseDuration);

        // 파티클 정지
        StopSparkle();

        // Draw 모션 종료
        swordAnimator.SetBool("WeaponPull", false);
        swordAnimator.Play("Idle Walk Run Blend", 0, 0f);

        isDrawing = false;
    }

    /// <summary>
    /// R 키로만 실행.
    /// 1) RaiseSword → 파티클 → LowerSword
    /// 2) 리듬 체크 실패/성공 판정
    /// 3) 성공 시 내구도 복구, 실패 시 아무 일도 없음
    /// </summary>
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

    // 공격/재장전 로직은 여기서 처리하지 않음
    protected override void Shoot() { /* empty */ }
    protected override void Reload() { /* empty */ }
}
