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
            bullet.transform.localScale = bulletScale; // ũ�� ����
            bullet.GetComponent<Rigidbody>().AddForce(bulletSpeed); // ���� �ο�
        }
        
    }

    Vector3 GetSpreadDirection()
    {
        // �⺻ ����
        Vector3 forward = shootPoint.forward;

        // ���� ����
        float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);

        // �����̼��� ����� ȸ�� ����
        Quaternion spreadRotation = Quaternion.Euler(y, x, 0);
        return spreadRotation * forward;
    }
}
