using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;

    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform shootPoint = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.Mouse1)) // 우클릭 입력 중
        {
            Zoom(); // 줌
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) // 좌클릭 입력
        {
            Shoot(); // 발사
        }

        if (Input.GetKeyDown(KeyCode.R))  // R키 입력
        {
            Reload(); // 장전
        }
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (nowAmmo <= 0)
        {
            Reload();
            return;
        }

        nowAmmo -= shotAmount;

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

        anim.SetTrigger("Shoot");
    }

    protected override void Reload()
    {
        base.Reload();

        nowAmmo = maxAmmo;

        anim.SetTrigger("Reload");
    }

    private void Zoom()
    {
        Debug.Log("줌 상태입니다.");
    }
}
