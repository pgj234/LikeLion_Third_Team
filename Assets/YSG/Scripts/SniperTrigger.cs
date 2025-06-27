using UnityEngine;

public class SniperTrigger : MonoBehaviour
{
    private Sniper sniper;

    private void Awake()
    {
        sniper = GetComponentInParent<Sniper>();
    }

    private void ShootOver() => sniper.ShootOverEvent();
    private void ReloadOver() => sniper.ReloadOverEvent();


    private void ShootSound() => SoundManager.Instance.PlaySFX(SFX.SniperShoot);
}
