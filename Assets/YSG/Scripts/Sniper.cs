using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;

    [SerializeField] private int reload = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        //base.Update();

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

        //if (nowAmmo <= 0)
        //{
        //    Reload();
        //    return;
        //}

        nowAmmo -= shotAmount;

        anim.SetTrigger("Shoot");
    }

    protected override void Reload()
    {
        base.Reload();

        nowAmmo = maxAmmo;

        anim.SetInteger("Reload", reload++);
    }

    private void Zoom()
    {
    }

    private void ResetReload()
    {
        reload = 0;
        anim.SetInteger("Reload", reload);
    }
}
