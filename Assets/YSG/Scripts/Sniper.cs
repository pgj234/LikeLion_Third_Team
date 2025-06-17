using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;

    [Header("����")]
    [SerializeField] private GameObject scopeUI; // UI ���ذ�
    [SerializeField] private float zoomFOV = 30; // �� �� �þ߰�
    private float normalFOV;
    private bool isZoomed = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        nowAmmo = maxAmmo;

        normalFOV = Camera.main.fieldOfView;

        if (scopeUI != null)
            scopeUI.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (isActing) return;

        // ����ĳ��Ʈ
        Debug.DrawRay(shootPoint.position, shootPoint.forward * 100, Color.red);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Zoom();
        }
        else
        {
            Unzoom();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Shoot");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (isActing) return;

        isActing = true;

        SoundManager.Instance.PlaySFX(SFX.SniperShoot);

        if (nowAmmo <= 0)
        {
            Reload();
            return;
        }

        nowAmmo -= shotAmount;

        // ���� �߻� �浹 üũ
        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("�浹 ���: " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("����");
            }
        }
        else
        {
            Debug.Log("������");
        }
    }

    protected override void Reload()
    {
        base.Reload();

        nowAmmo = maxAmmo;

        if (GameManager.Instance.RhythmCheck() > 0)
        {
            Debug.Log("���� ����");

            anim.SetInteger("Reload", reload++);
        }
        else
        {
            Debug.Log("���� ����");

            reload = 0;
            anim.SetInteger("Reload", reload);
        }
    }

    private void Zoom()
    {
        if (!isZoomed)
        {
            Camera.main.fieldOfView = zoomFOV;
            if (scopeUI != null) scopeUI.SetActive(true);
            isZoomed = true;
        }
    }

    private void Unzoom()
    {
        Camera.main.fieldOfView = normalFOV;
        if (scopeUI != null) scopeUI.SetActive(false);
        isZoomed = false;
    }

    private void ActOver() => isActing = false;

    private void ReloadOver()
    {
        Debug.Log("���� ����");

        SoundManager.Instance.PlaySFX(SFX.RhythmFail);

        reload = 0;
        anim.SetInteger("Reload", reload);
    }
}
