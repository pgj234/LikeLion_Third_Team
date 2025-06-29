using UnityEngine;

public class SniperTrigger : MonoBehaviour
{
    private Sniper sniper;

    private void Awake()
    {
        sniper = GetComponentInParent<Sniper>();
    }

    #region �ִϸ��̼�
    private void ShootOver() => sniper.ShootOverEvent();
    private void ReloadOver() => sniper.ReloadOverEvent();
    #endregion

    #region ����
    private void ShootSound() => SoundManager.Instance.PlaySFX(SFX.SniperShoot);
    private void ReloadSound(int num)
    {
        switch (num)
        {
            case 1:
                SoundManager.Instance.PlaySFX(SFX.SniperReload1);
                break;
            case 2:
                SoundManager.Instance.PlaySFX(SFX.SniperReload2);
                break;
            case 3:
                SoundManager.Instance.PlaySFX(SFX.SniperReload3);
                break;
            case 4:
                SoundManager.Instance.PlaySFX(SFX.SniperReload4);
                break;
        }
    }
    #endregion
}
