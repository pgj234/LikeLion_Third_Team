using System;
using UnityEngine;

public class EventManager : SingletonBehaviour<EventManager>
{
    // 플레이어
    // 데미지 이벤트
    public Action<int> OnPlayerDamageAction;

    // 플레이어 사망 이벤트
    public Action OnPlayerDieAction;
    // 부활 이벤트
    public Action OnPlayerRevivalAction;

    // 콤보 증가
    public Action OnPlayerAddComboAction;
    // 콤보 감소
    public Action OnPlayerReduceComboAction;

    protected override void Init()
    {
        base.Init();
    }

    // 플레이어 데미지 이벤트 발생 메서드
    public void PlayerDamageEvent(int damage)
    {
        OnPlayerDamageAction?.Invoke(damage);
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

    // 플레이어 콤보 증가 이벤트 실행 메서드
    public void PlayerAddComboEvent()
    {
        OnPlayerAddComboAction?.Invoke();
    }

    // 플레이어 콤보 증가 이벤트 실행 메서드
    public void PlayerReduceComboEvent()
    {
        OnPlayerReduceComboAction?.Invoke();
    }
}
