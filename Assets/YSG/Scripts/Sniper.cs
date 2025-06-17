using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;

    [Header("조준")]
    [SerializeField] private GameObject scopeUI; // UI 조준경
    [SerializeField] private float zoomFOV = 30; // 줌 시 시야각
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

        // 레이캐스트
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

        // 실제 발사 충돌 체크
        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("충돌 대상: " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("명중");
            }
        }
        else
        {
            Debug.Log("빗나감");
        }
    }

    protected override void Reload()
    {
        base.Reload();

        nowAmmo = maxAmmo;

        if (GameManager.Instance.RhythmCheck() > 0)
        {
            Debug.Log("장전 성공");

            anim.SetInteger("Reload", reload++);
        }
        else
        {
            Debug.Log("장전 실패");

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
        Debug.Log("장전 성공");

        SoundManager.Instance.PlaySFX(SFX.RhythmFail);

        reload = 0;
        anim.SetInteger("Reload", reload);
    }
}
