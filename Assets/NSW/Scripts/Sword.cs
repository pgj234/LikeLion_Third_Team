using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sword : WeaponBase
{
    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;      // Draw Sword 2 애니 길이
    [SerializeField] float sparkleDuration = 0.3f;      // 파티클 유지 시간
    [SerializeField] float reloadLowerDuration = 0.5f;      // Put 애니 길이

    [Header("Particle Settings")]
    [SerializeField] List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();

    [Header("Animator Settings")]
    [SerializeField] Animator swordAnimator;

    bool isDrawing = false;

    void Awake()
    {
        // Sparkle 프리팹 인스턴스 생성
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

        if (Input.GetKeyDown(KeyCode.R) && !isDrawing)
            StartCoroutine(DrawSwordSequence());
    }

    IEnumerator DrawSwordSequence()
    {
        isDrawing = true;

        // 1) Draw Sword 애니 + 파티클
        swordAnimator.SetBool("WeaponPull", true);
        swordAnimator.Play("Draw Sword 2", 0, 0f);
        PlaySparkleOnce();

        // 2) 두 번째 R키 입력 대기
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        // 3) 리듬 체크
        int timing = GameManager.Instance?.RhythmCheck() ?? 1;
        if (timing == 0)
        {
            Debug.Log("리듬 실패: Draw 단계 → 바로 Put 애니로");
            // 실패 시 파티클 정리
            StopSparkle();

            // 실패 처리용 Put 애니
            swordAnimator.SetBool("WeaponPull", false);
            swordAnimator.SetBool("WeaponPut", true);
            yield return new WaitForSeconds(reloadLowerDuration);
            swordAnimator.SetBool("WeaponPut", false);

            isDrawing = false;
            yield break;
        }

        Debug.Log($"리듬 성공({timing}): Draw 단계 → Hold/Put 이어서 진행");

        // 4) 성공 시 sparkle 유지
        yield return new WaitForSeconds(sparkleDuration);

        // 5) Hold 애니(Idle 상태) 없이 바로 Put 애니로 넘어가도 되고,
        //    필요하면 쇼트 핸드 Idle을 먼저 재생할 수 있습니다.
        swordAnimator.SetBool("WeaponPull", false);
        swordAnimator.SetBool("WeaponPut", true);
        swordAnimator.Play("hand Idle", 0, 0f);

        // 6) Put 애니 대기
        yield return new WaitForSeconds(reloadLowerDuration);

        // 7) 시퀀스 끝
        swordAnimator.SetBool("WeaponPut", false);
        swordAnimator.Play("Idle Walk Run Blend", 0, 0f);

        isDrawing = false;
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

    protected override void Shoot() { /* empty */ }
    protected override void Reload() { /* empty */ }
}
