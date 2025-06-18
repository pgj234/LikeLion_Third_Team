using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int weaponNum;                // 무기 번호 (스왑용으로도 사용)
    [SerializeField] protected int nowAmmo;                  // 발사준비된 탄환
    [SerializeField] protected int maxAmmo;                  // 재장전시 탄환 최대치 / -1 무제한
    [SerializeField] protected int shotAmount;               // 발사시 나가는 탄환수
    [SerializeField] protected float nextShotTime;           // 다음 발사까지 딜레이시간
    [SerializeField] protected float shotDamage;             // 탄 데미지

    [SerializeField] protected Vector2 shotSpreadMin;        // 탄퍼짐 최솟값
    [SerializeField] protected Vector2 shotSpreadMax;        // 탄퍼짐 최댓값

    [SerializeField] protected int reloadStepNum;            // 장전 단계
    protected int currentReloadStepNum;     // 현재 장전 단계

    protected int rhythmTimingNum;          // 0 : 박자 타이밍 X, 1 : 정박 타이밍, 2 : 반박 타이밍

    internal bool reloading = false;

    protected virtual void Update()
    {
        rhythmTimingNum = GameManager.Instance.RhythmCheck();
    }

    protected virtual void Shoot()         // 발사 메서드
    {

    }

    protected virtual void Reload()        // 재장전 메서드
    {
        reloading = true;
    }
}
