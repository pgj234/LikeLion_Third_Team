using System;
using UnityEngine;

public class EventManager : SingletonBehaviour<EventManager>
{
    // 스코어링 이벤트
    public Action<int> OnScoreRefreshAction;

    // Pause 윈도우 오픈 여부 이벤트
    public Action<bool> OnPauseAction;

    // 플레이어

    // 플레이어 최대 체력
    public Action<int> OnPlayerMaxHpAction;
    // 데미지 이벤트 (플레이어 체력 UI 표시)
    public Action<int> OnPlayerDamageAction;
    // 플레이어 사용가능 무기 UI 새로고침 이벤트
    public Action<bool[]> OnPlayerWeaponUIRefreshAction;
    // 플레이어 현재 장전 탄 수 UI 새로고침 이벤트
    public Action<int> OnPlayerCurrentBulletUIRefreshAction;
    // 플레이어 최대 장전 탄 수 UI 새로고침 이벤트
    public Action<int> OnPlayerMaxBulletUIRefreshAction;

    // 플레이어 사망 이벤트
    public Action OnPlayerDieAction;
    // 부활 이벤트
    public Action OnPlayerRevivalAction;

    // 콤보 새로고침 이벤트
    public Action<int> OnPlayerComboRefreshAction;

    protected override void Init()
    {
        base.Init();
    }

    // Puase 윈도우 오픈 여부
    public void PauseWindowOpen(bool isOpen)
    {
        OnPauseAction?.Invoke(isOpen);
    }

    // 스코어링 이벤트 발생 메서드
    public void ScoreRefreshEvent(int score)
    {
        OnScoreRefreshAction?.Invoke(score);
    }

    // 플레이어 최대 체력
    public void PlayerMaxHpEvent(int maxHp)
    {
        OnPlayerMaxHpAction?.Invoke(maxHp);
    }

    // 플레이어 데미지 이벤트 발생 메서드
    public void PlayerDamageEvent(int damage)
    {
        OnPlayerDamageAction?.Invoke(damage);
    }

    // 플레이어 사용가능 무기 UI 새로고침
    public void PlayerWeaponUIRefresh(bool[] weaponUseAbleArray)
    {
        OnPlayerWeaponUIRefreshAction?.Invoke(weaponUseAbleArray);
    }

    // 플레이어 사망 이벤트 실행 메서드
    public void PlayerDieEvent()
    {
        OnPlayerDieAction?.Invoke();
    }

    // 플레이어 부활 이벤트 실행 메서드
    public void PlayerRevivalEvent()
    {
        OnPlayerRevivalAction?.Invoke();
    }

    // 플레이어 콤보 새로고침 이벤트 실행 메서드
    public void PlayerComboRefreshEvent(int combo)
    {
        OnPlayerComboRefreshAction?.Invoke(combo);
    }

    // 플레이어 현재 장전 탄 수 UI 새로고침 이벤트
    public void PlayerCurrentBulletUIRefresh(int currentBullet)
    {
        OnPlayerCurrentBulletUIRefreshAction?.Invoke(currentBullet);
    }

    // 플레이어 최대 장전 탄 수 UI 새로고침 이벤트
    public void PlayerMaxBulletUIRefresh(int maxBullet)
    {
        OnPlayerMaxBulletUIRefreshAction?.Invoke(maxBullet);
    }
}
