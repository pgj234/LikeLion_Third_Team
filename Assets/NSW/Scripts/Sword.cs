using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sword : WeaponBase
{
    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;
    [SerializeField] float sparkleDuration = 0.3f;
    [SerializeField] float reloadLowerDuration = 0.5f;

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Animator Settings")]
    [SerializeField] Animator swordAnimator;

    bool isDrawing = false;
    bool isReloading = false;

    void Awake()
    {
        // 파티클 인스턴스 생성
        foreach (var prefab in sparklePrefabs)
            if (prefab != null)
            {
                var inst = Instantiate(prefab, transform);
                inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                sparkleInstances.Add(inst);
            }
    }

    protected override void Update()
    {
        base.Update();

        // R키로 Draw 시퀀스
        if (Input.GetKeyDown(KeyCode.R) && !isDrawing && !isReloading)
            StartCoroutine(DrawSwordSequence());
    }

    IEnumerator DrawSwordSequence()
    {
        isDrawing = true;
        isReloading = true;

        // 1) Draw 애니 + 파티클
        swordAnimator.Play("Draw Sword 2", 0, 0f);
        PlaySparkleOnce();

        // 애니 길이만큼 대기
        yield return new WaitForSeconds(reloadRaiseDuration);

        // 2) 리듬 체크
        int timing = GameManager.Instance != null
                     ? GameManager.Instance.RhythmCheck()
                     : 1;

        if (timing == 0)
        {
            Debug.Log("리듬 실패: Draw 단계 → Idle로 되돌아갑니다.");
            // 실패 시 파티클 정리
            StopSparkle();
            // Idle로 전환
            swordAnimator.Play("Idle Walk Run Blend", 0, 0f);
            // 상태 리셋
            isDrawing = false;
            isReloading = false;
            yield break;
        }

        Debug.Log($"리듬 성공({timing}): Draw 단계");

        // 3) 성공 시 sparkle 유지
        yield return new WaitForSeconds(sparkleDuration);

        // 4) Hold 애니 (Draw → Hold 연결)
        swordAnimator.Play("hand Idle", 0, 0f);

        // Hold 애니 길이만큼 대기
        yield return new WaitForSeconds(reloadLowerDuration);

        // 5) 최종 Idle-Walk-Run Blend 복귀
        swordAnimator.Play("Idle Walk Run Blend", 0, 0f);

        // 상태 리셋
        isDrawing = false;
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

    protected override void Shoot() { }
    protected override void Reload() { }
}
