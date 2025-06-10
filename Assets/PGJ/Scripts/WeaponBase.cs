using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected int nowAmmo;                  // 발사준비된 탄환
    protected int maxAmmo;                  // 재장전시 탄환 최대치 / -1 무제한
    protected int shotAmount;               // 발사시 나가는 탄환수
    protected float nextShotTime;           // 다음 발사까지 딜레이시간
    protected float reloadTime;             // 재장전시간
    protected float shotDamage;             // 탄 데미지

    protected int reloadStepNum;            // 장전 단계

    protected bool rhythmOK;                // true면 리듬 타이밍O, false면 리듬 타이밍X

    protected virtual void Update()
    {
        rhythmOK = GameManager.Instance.RhythmCheck();
    }

    protected virtual void Shoot()         // 발사 메서드
    {

    }

    protected virtual void Reload()        // 재장전 메서드
    {

    }
}
