using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sword weapon script that handles swinging, reloading, sparkle effects, hit detection,
/// and rhythm-based draw phases. Press R to reload and replay sparkle.
/// </summary>
public class Sword : WeaponBase
{
    [Header("Sword Settings")]
    [SerializeField] protected int initialDurability = 100;
    [SerializeField] protected int durabilityPerSwing = 1;
    [SerializeField] protected float swingCooldown = 0.5f;
    [SerializeField] protected float reloadRaiseDuration = 0.5f;
    [SerializeField] protected float sparkleDuration = 0.3f;
    [SerializeField] protected float reloadLowerDuration = 0.5f;
    [SerializeField] protected int reloadShot = 100;

    [Header("Particle Settings")]
    [Tooltip("Particle System Prefabs for sparkle effect")]
    [SerializeField] protected List<ParticleSystem> sparklePrefabs;
    private List<ParticleSystem> sparkleInstances = new List<ParticleSystem>();
    [SerializeField] protected int sparkleCount = 30;

    [Header("Hit Detection")]
    [SerializeField] protected float hitRange = 5f;
    [SerializeField] protected LayerMask targetLayer;

    [Header("References")]
    [SerializeField] protected Animator swordAnimator;

    private bool isReloading = false;

    // Public durations for external use
    public float ReloadRaiseDuration => reloadRaiseDuration;
    public float SparkleDuration => sparkleDuration;
    public float ReloadLowerDuration => reloadLowerDuration;

    private void Awake()
    {
        // Initialize WeaponBase fields
        maxAmmo = initialDurability;
        nowAmmo = initialDurability;
        shotAmount = durabilityPerSwing;
        nextShotTime = 0f;

        // Instantiate sparkle systems as children and stop them
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

    protected override void Update()
    {
        // Rhythm timing update
        if (GameManager.Instance != null)
            base.Update();

        // Reload on R key press
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Trigger reload routine
            Reload();
        }
    }

    /// <summary>
    /// Swing action: deduct ammo, play animation, sparkle, and detect hits.
    /// </summary>
    public void Swing()
    {
        if (isReloading || Time.time < nextShotTime || nowAmmo < shotAmount)
            return;

        nowAmmo -= shotAmount;
        nextShotTime = Time.time + swingCooldown;

        swordAnimator?.SetTrigger("Swing");
        PlaySparkleOnce();

        var cam = Camera.main;
        if (cam != null && Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, hitRange, targetLayer))
        {
            Debug.Log($"Hit target: {hit.collider.name} at {hit.point}");
            if (hit.collider.GetComponent<IDamageable>() is IDamageable dmg)
                dmg.TakeDamage(shotDamage);
        }
        else
        {
            Debug.Log("Swing missed");
        }
    }

    protected override void Shoot() => Swing();

    protected override void Reload()
    {
        if (isReloading || nowAmmo >= maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // Rhythm check before draw step
        int drawTiming = GameManager.Instance?.RhythmCheck() ?? 0;
        switch (drawTiming)
        {
            case 1: Debug.Log("정박 타이밍! Draw Sword 성공"); break;
            case 2: Debug.Log("반박 타이밍! Draw Sword 보조 성공"); break;
            default: Debug.Log("박자 실패: Draw Sword 단계"); break;
        }

        // Play draw animation and sparkle simultaneously
        swordAnimator?.SetTrigger("RaiseSword");
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Play();
        }

        // Keep drawn state for 3 seconds
        yield return new WaitForSeconds(3f);

        // Stop sparkle
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Clear();
        }

        // Rhythm check before lower step
        int lowerTiming = GameManager.Instance?.RhythmCheck() ?? 0;
        switch (lowerTiming)
        {
            case 1: Debug.Log("정박 타이밍! Lower Sword 성공"); break;
            case 2: Debug.Log("반박 타이밍! Lower Sword 보조 성공"); break;
            default: Debug.Log("박자 실패: Lower Sword 단계"); break;
        }

        // Play lower animation
        swordAnimator?.SetTrigger("LowerSword");
        yield return new WaitForSeconds(reloadLowerDuration);

        // Restore ammo and finish reload
        nowAmmo = Mathf.Clamp(reloadShot, 0, maxAmmo);
        isReloading = false;
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");
    }

    /// <summary>
    /// Play sparkle systems once according to burst settings.
    /// </summary>
    public void PlaySparkleOnce()
    {
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Play();
        }
        Invoke(nameof(StopSparkle), sparkleDuration);
    }

    public void StopSparkle()
    {
        foreach (var inst in sparkleInstances)
        {
            inst.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            inst.Clear();
        }
    }
}
