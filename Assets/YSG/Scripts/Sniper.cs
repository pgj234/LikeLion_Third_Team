using UnityEngine;

// TODO
// 1. ���� ��� �� ����
// 2. �߻� ��� �� ���� + �� �ǰ� ���� Ȯ�� + Ÿ�̹� üũ
// 3. ���� ��� + Ÿ�̹� üũ

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
