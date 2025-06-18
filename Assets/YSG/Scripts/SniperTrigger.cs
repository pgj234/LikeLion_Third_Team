using UnityEngine;

public class SniperTrigger : MonoBehaviour
{
    private Sniper sniper;

    private void Awake()
    {
        sniper = GetComponentInParent<Sniper>();
    }

    private void Shoot() => sniper.ShootEvent();
    private void ActOver() => sniper.ActOverEvent();
    private void ReloadOver() => sniper.ReloadOverEvent();
}
