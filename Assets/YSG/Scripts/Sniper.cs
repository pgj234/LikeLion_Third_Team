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
        //base.Update();

        if (Input.GetKey(KeyCode.Mouse1)) // ��Ŭ�� �Է� ��
        {
            Zoom(); // ��
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) // ��Ŭ�� �Է�
        {
            Shoot(); // �߻�
        }

        if (Input.GetKeyDown(KeyCode.R))  // RŰ �Է�
        {
            Reload(); // ����
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

        anim.SetTrigger("Reload");
    }

    private void Zoom()
    {
    }
}
