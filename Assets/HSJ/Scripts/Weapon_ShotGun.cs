using UnityEngine;

public class Weapon_ShotGun : WeaponBase
{
    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] Vector3 bulletSpeed = Vector3.one;
    [SerializeField] Transform shootPoint = null;

    protected override void Update()
    {
        base.Update();
    }

    protected override void Reload()
    {
        base.Reload();
    }

    protected override void Shoot()
    {
        base.Shoot();

        for(int i=0;i< shotAmount; i++)
        {
            Vector3 dir = GetSpreadDirection();
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, new Quaternion(dir.x, dir.y, dir.z, transform.rotation.w));
            bullet.transform.localScale = bulletScale; // 크기 설정
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpeed); // 힘을 부여
        }
        
    }

    Vector3 GetSpreadDirection()
    {
        // 기본 방향
        Vector3 forward = shootPoint.forward;

        // 퍼짐 적용
        float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);

        // 로테이션을 만들고 회전 적용
        Quaternion spreadRotation = Quaternion.Euler(y, x, 0);
        return spreadRotation * forward;
    }
}
