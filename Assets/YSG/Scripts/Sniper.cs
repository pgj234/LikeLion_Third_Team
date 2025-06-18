using System.Collections;
using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;
    private InputManager input;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;

    [Header("조준")]
    [SerializeField] private GameObject scopeUI; // UI 조준경
    [SerializeField] private float zoomFOV = 30; // 줌 시 시야각
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private float normalFOV;
    private bool isZoomed = false;

    [Header("이펙트")]
    [SerializeField] private GameObject shootFire;
    [SerializeField] private GameObject ShootTrail;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        input = InputManager.Instance;

        nowAmmo = maxAmmo;

        scopeUI?.SetActive(false);
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

        // 총구 화염 생성
        if (shootFire != null)
        {
            GameObject fire = Instantiate(shootFire, shootPoint.position, shootPoint.rotation, shootPoint);
            Destroy(fire, 0.1f);
        }

        // 총알 궤적 생성
        if (ShootTrail != null)
        {
            GameObject trail = Instantiate(ShootTrail, shootPoint.position, shootPoint.rotation);
            if (trail.TryGetComponent(out TrailRenderer trailRenderer))
            {
                trailRenderer.Clear();
            }

            // 궤적을 충돌 지점까지 이동시킴
            Vector3 endPos = shootPoint.position + shootPoint.forward * 100;

            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit2, 100))
            {
                endPos = hit2.point;
            }

            StartCoroutine(MoveTrail(trail.transform, endPos));
        }

        // 충돌 판정
        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("충돌 대상 : " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("명중");
            }
        }
        else
        {
            Debug.Log("빗나감");
        }

        Unzoom();
    }

    private IEnumerator MoveTrail(Transform _trail, Vector3 _targetPos)
    {
        float time = 0f;
        Vector3 start = _trail.position;
        float duration = 0.1f;

        while (time < duration)
        {
            _trail.position = Vector3.Lerp(start, _targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        _trail.position = _targetPos;
        Destroy(_trail.gameObject, 0.5f);
    }

    protected override void Reload()
    {
        base.Reload();

        nowAmmo = maxAmmo;

        if (GameManager.Instance.RhythmCheck() > 0)
        {
            Debug.Log("박자 성공");

            reload++;
        }
        else
        {
            Debug.Log("박자 실패");

            reload = 0;
        }

        anim.SetInteger("Reload", reload);
    }

    private void Zoom()
    {
        if (!isZoomed)
        {
            Camera.main.fieldOfView = zoomFOV;
            Camera.main.transform.position = shootPoint.position;
            Camera.main.transform.rotation = shootPoint.rotation;

            scopeUI?.SetActive(true);
            isZoomed = true;
        }
    }

    private void Unzoom()
    {
        Camera.main.fieldOfView = normalFOV;
        Camera.main.transform.position = originalCamPos;
        Camera.main.transform.rotation = originalCamRot;

        scopeUI?.SetActive(false);
        isZoomed = false;
    }

    #region 애니메이션 이벤트
    public void ShootEvent() => Shoot();

    public void ActOverEvent() => isActing = false;

    public void ReloadOverEvent()
    {
        Debug.Log("장전 성공");

        reload = 0;
        anim.SetInteger("Reload", reload);
    }
    #endregion
}
