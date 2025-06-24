using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sword : WeaponBase
{
    [Header("Draw Settings")]
    [SerializeField] float reloadRaiseDuration = 0.5f;      // Draw Sword 2 애니 길이 (실제 애니 길이와 맞출 것)
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

        if (Input.GetKeyDown(KeyCode.R) && !isDrawing)
        {
            StartCoroutine(DrawSwordSequence());
        }
    }

    IEnumerator DrawSwordSequence()
    {
        isDrawing = true;

        // 1) Draw Sword 2 애니 재생
        swordAnimator.SetBool("WeaponPull", true);
        swordAnimator.Play("Draw Sword 2", 0, 0f);

        // 2) 파티클 팡!
        PlaySparkleOnce();

        // ── 여기서 딱 멈춤 ──
        // 다음 단계로 넘어가려면 다시 R키를 눌러야 합니다.
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.R));

        // (선택) 두 번째 R키에서도 리듬 체크를 하고 싶으면 여기에 추가
        int timing = GameManager.Instance?.RhythmCheck() ?? 1;
        if (timing == 0) { /* 실패 처리 */ }

        // 3) 파티클 유지 시간 만큼 대기
        yield return new WaitForSeconds(sparkleDuration);

        // 4) Hold 애니(WeaponPut)로 전환
        swordAnimator.SetBool("WeaponPull", false);
        swordAnimator.SetBool("WeaponPut", true);
        swordAnimator.Play("hand Idle", 0, 0f);

        // 5) Put 애니 길이 만큼 대기
        yield return new WaitForSeconds(reloadLowerDuration);

        // 6) 상태 초기화
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
