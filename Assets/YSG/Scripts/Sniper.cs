using System.Collections;
using UnityEngine;

public class Sniper : WeaponBase
{
    private Animator anim;
    private InputManager input;

    private bool isActing = true;
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;

    [Header("����")]
    [SerializeField] private GameObject scopeUI; // UI ���ذ�
    [SerializeField] private float zoomFOV = 30; // �� �� �þ߰�
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private float normalFOV;
    private bool isZoomed = false;

    [Header("����Ʈ")]
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

        if (GameManager.Instance.RhythmCheck() > 0)
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
