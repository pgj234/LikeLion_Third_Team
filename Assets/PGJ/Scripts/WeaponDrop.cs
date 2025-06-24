using UnityEngine;

enum WeaponDropKind
{
    Sword,
    Pistol,
    ShotGun,
    Sniper
}

public class WeaponDrop : MonoBehaviour
{
    [SerializeField] float yRotateSpd;
    [SerializeField] WeaponDropKind weaponDropKind;

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            if (col.transform.TryGetComponent(out Player player))
            {
                SoundManager.Instance.PlaySFX(SFX.WeaponGet);

                player.GetWeapon((int)weaponDropKind);
                Destroy(gameObject);
            }            
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0) * yRotateSpd);
    }
}