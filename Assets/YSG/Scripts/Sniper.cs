using System.Collections;
using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;
    private InputManager input;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;
    private Vector3 originalShootPointLocalPos;
    private Quaternion originalShootPointLocalRot;


    [Header("����")]
    [SerializeField] private GameObject scopeUI;
    [SerializeField] private float zoomScale = 4;
    private bool isZooming = false;

    [Header("����Ʈ")]
    [SerializeField] private GameObject shootFire;
    [SerializeField] private GameObject ShootTrail;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        input = InputManager.Instance;

        nowAmmo = maxAmmo;

        scopeUI?.SetActive(false);

        originalShootPointLocalPos = shootPoint.localPosition;
        originalShootPointLocalRot = shootPoint.localRotation;
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
            {
                Zoom();
            }
            else
            {
                Unzoom();
            }
        }

        if (input.mouse0_Input)
        {
            input.mouse0_Input = false;
            anim.SetTrigger("Shoot");
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

        // �ѱ� ȭ�� ����
        if (shootFire != null)
        {
            GameObject fire = Instantiate(shootFire, shootPoint.position, shootPoint.rotation, shootPoint);
            Destroy(fire, 0.1f);
        }

        // �Ѿ� ���� ����
        if (ShootTrail != null)
        {
            GameObject trail = Instantiate(ShootTrail, shootPoint.position, shootPoint.rotation);
            if (trail.TryGetComponent(out TrailRenderer trailRenderer))
            {
                trailRenderer.Clear();
            }

            // ������ �浹 �������� �̵���Ŵ
            Vector3 endPos = shootPoint.position + shootPoint.forward * 100;

            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit2, 100))
            {
                endPos = hit2.point;
            }

            StartCoroutine(MoveTrail(trail.transform, endPos));
        }

        // �浹 ����
        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.Log("�浹 ��� : " + hit.collider.name);

            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("����");

                hit.collider.GetComponent<Monster>()?.Hit(shotDamage);
            }
        }
        else
        {
            Debug.Log("������");
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

        if (GameManager.Instance.RhythmCheck() > 0
            || true) // �ӽ�
        {
            Debug.Log("���� ����");

            reload++;
        }
        else
        {
            Debug.Log("���� ����");

            reload = 0;
        }

        anim.SetInteger("Reload", reload);
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

    #region �ִϸ��̼� �̺�Ʈ
    public void ShootEvent() => Shoot();

    public void ActOverEvent() => isActing = false;

    public void ReloadOverEvent()
    {
        Debug.Log("���� ����");

        reload = 0;
        anim.SetInteger("Reload", reload);
    }
    #endregion
}
