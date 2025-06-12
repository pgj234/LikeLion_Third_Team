using UnityEngine;
using System.Collections;

public class Sword : WeaponBase
{
    [Header("Sword Settings")]
    [SerializeField] protected int initialDurability = 100;
    [SerializeField] protected int durabilityPerSwing = 1;
    [SerializeField] protected float swingCooldown = 0.5f;
    [SerializeField] protected float reloadRaiseDuration = 0.5f;   // Phase 1 애니메이션 길이
    [SerializeField] protected float sparkleDuration = 0.3f;   // 반짝임 지속 시간
    [SerializeField] protected float reloadLowerDuration = 0.5f;   // Phase 2 애니메이션 길이
    [SerializeField] protected int reloadShot = 100;

    [Header("References")]
    [SerializeField] protected Animator swordAnimator;
    [SerializeField] protected ParticleSystem sparkleEffect;

    protected bool isReloading = false;

    protected void Awake()
    {
        maxAmmo = initialDurability;
        nowAmmo = initialDurability;
        shotAmount = durabilityPerSwing;
        nextShotTime = 0f;
    }

    public void Swing()
    {
        if (isReloading || Time.time < nextShotTime || nowAmmo < shotAmount) return;

        nowAmmo -= shotAmount;
        nextShotTime = Time.time + swingCooldown;
        swordAnimator?.SetTrigger("Swing");
        // TODO: 데미지 판정
    }

    protected override void Shoot() => Swing();

    protected override void Reload()
    {
        if (isReloading || nowAmmo >= maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    protected IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // === Phase 1: 올려서 반짝이기 ===
        swordAnimator?.SetTrigger("RaiseSword");
        yield return new WaitForSeconds(reloadRaiseDuration);

        if (sparkleEffect != null)
            sparkleEffect.Play();
        yield return new WaitForSeconds(sparkleDuration);

        // === Phase 2: 내리기 및 완료 ===
        swordAnimator?.SetTrigger("LowerSword");
        yield return new WaitForSeconds(reloadLowerDuration);

        nowAmmo = Mathf.Clamp(reloadShot, 0, maxAmmo);
        isReloading = false;
        Debug.Log($"Reload Complete! Durability: {nowAmmo}");
    }
}
