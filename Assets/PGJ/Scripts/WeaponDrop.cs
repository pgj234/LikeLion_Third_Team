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
    [SerializeField] WeaponDropKind weaponDropKind;

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            if (col.transform.TryGetComponent(out Player player))
            {
                player.GetWeapon((int)weaponDropKind);
                Destroy(gameObject);
            }            
        }
    }
}