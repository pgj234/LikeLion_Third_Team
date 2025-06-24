using System.Collections;
using UnityEngine;

public class Sniper : WeaponBase
{
    private InputManager input;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;
    private Vector3 originalShootPointLocalPos;
    private Quaternion originalShootPointLocalRot;

    [Header("발사")]
    [SerializeField] private float spreadAngle = 3;

    [Header("조준")]
    [SerializeField] private GameObject scopeUI;
    [SerializeField] private float zoomScale = 4;
    private bool isZooming = false;

    [Header("이펙트")]
    [SerializeField] private GameObject shootFire;
    [SerializeField] private LineRenderer shootTrail;
    [SerializeField] private float trailDuration = 0.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        input = InputManager.Instance;

        nowAmmo = maxAmmo;

        scopeUI?.SetActive(false);

        originalShootPointLocalPos = shootPoint.localPosition;
        originalShootPointLocalRot = shootPoint.localRotation;

        shootTrail = GetComponent<LineRenderer>();
        if (shootTrail != null) shootTrail.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        if (isActing) return;

        Debug.DrawRay(shootPoint.position, shootPoint.forward * 100, Color.red);

        if (input.mouse1_Input)
        {
            input.mouse0_Input = false;

            if (!isZooming)
                Zoom();
            else
                Unzoom();
        }

        if (input.mouse0_Input)
        {
            input.mouse0_Input = false;
            anim.SetTrigger("WeaponAttack");
        }

        if (input.r_Input)
        {
            input.r_Input = false;
            Reload();
        }

        if (isZooming)
        {
            shootPoint.rotation = Camera.main.transform.rotation;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (isActing) return;

        if (nowAmmo <= 0)
        {
            Reload();
            return;
        }

        isActing = true;

        SoundManager.Instance.PlaySFX(SFX.SniperShoot);

        nowAmmo -= shotAmount;

        if (shootFire != null)
        {
            GameObject fire = Instantiate(shootFire, shootPoint.position, shootPoint.rotation, shootPoint);
            Destroy(fire, 0.1f);
        }

        Vector3 shootDir = shootPoint.forward;

        Ray ray = new Ray(shootPoint.position, shootDir);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("충돌 대상 : " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("명중");
                hit.collider.GetComponent<Monster>()?.Hit(shotDamage);
            }

            DrawTrail(shootPoint.position, hit.point); 
        }
        else
        {
            Debug.Log("빗나감");

            Vector3 missPoint = shootPoint.position + shootDir * 100;
            DrawTrail(shootPoint.position, missPoint); 
        }

        Unzoom();
    }

    protected override void Reload()
    {
        base.Reload();

        if (isActing) return;

        nowAmmo = maxAmmo;

        if (GameManager.Instance.RhythmCheck() > 0
            || true) // 임시
        {
            Debug.Log("박자 성공");

            reload++;
        }
        else
        {
            Debug.Log("박자 실패");

            reload = 0;
        }

        anim.SetInteger("WeaponReload", reload);
    }

    private void Zoom()
    {
        if (isZooming) return;

        Camera.main.fieldOfView /= zoomScale;

        shootPoint.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        shootPoint.rotation = Camera.main.transform.rotation;

        scopeUI?.SetActive(true);
        isZooming = true;
    }

    private void Unzoom()
    {
        if (!isZooming) return;

        Camera.main.fieldOfView *= zoomScale;

        shootPoint.localPosition = originalShootPointLocalPos;
        shootPoint.localRotation = originalShootPointLocalRot;

        scopeUI?.SetActive(false);
        isZooming = false;
    }

    private void DrawTrail(Vector3 _start, Vector3 _end)
    {
        if (shootTrail == null) return;

        shootTrail.positionCount = 2;
        shootTrail.SetPosition(0, _start);
        shootTrail.SetPosition(1, _end);
        shootTrail.enabled = true;

        StartCoroutine(DisableTrail());
    }

    private IEnumerator DisableTrail()
    {
        yield return new WaitForSeconds(trailDuration);
        shootTrail.enabled = false;
    }

    #region 애니메이션
    public void ShootEvent() => Shoot();

    public void ActOverEvent() => isActing = false;

    public void ReloadOverEvent()
    {
        Debug.Log("장전 성공");

        reload = 0;
        anim.SetInteger("Reload", reload);
    }

    public void SetAnimSpeed(float speed)
    {
        if (anim == null) return;
        anim.speed = speed;
    }

    public void ResetAnimSpeed()
    {
        if (anim == null) return;
        anim.speed = 1;
    }
    #endregion
}
