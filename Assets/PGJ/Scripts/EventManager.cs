using System;
using UnityEngine;

public class EventManager : SingletonBehaviour<EventManager>
{
    // 플레이어
    // 데미지 이벤트
    public Action<int> OnPlayerDamageAction;

    // 부활 이벤트
    public Action OnPlayerRevivalAction;

    // 콤보 증감
    public Action OnPlayerComboAction;

    protected override void Init()
    {
        base.Init();
    }

    // 플레이어 데미지 이벤트 발생 메서드
    public void PlayerDamageEvent(int damage)
    {
        OnPlayerDamageAction?.Invoke(damage);
    }

    // 플레이어 부활 이벤트 실행 메서드
    public void PlayerRevivalEvent()
    {
        OnPlayerRevivalAction?.Invoke();
    }

    public void PlayerComboEvent()
    {
        OnPlayerComboAction?.Invoke();
    }
}
