using UnityEngine;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] Vector3 bulletSpeed = Vector3.one;
    [SerializeField] Transform shootPoint = null;

    protected override void Shoot()
    {
        base.Shoot();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Reload()
    {
        base.Reload();
    }

}
