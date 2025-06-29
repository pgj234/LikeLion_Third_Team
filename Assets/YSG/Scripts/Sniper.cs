﻿using System.Collections;
using UnityEngine;

public class Sniper : WeaponBase
{
    [Header("발사")]
    [SerializeField] private int reload = 0;
    [SerializeField] private Transform shootPoint;
    private bool isShooting = false;

    [Header("조준")]
    [SerializeField] private GameObject scopeUI;
    [SerializeField] private float zoomScale = 4;
    private bool isZooming = false;

    private Camera cam;
    private Vector3 originCamPos;
    private Quaternion originCamRot;
    private Transform originCamParent;

    [Header("이펙트")]
    [SerializeField] private GameObject shootFire;
    [SerializeField] private LineRenderer shootTrail;
    [SerializeField] private float trailDuration = 0.5f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        nowAmmo = maxAmmo;

        scopeUI?.SetActive(false);

        cam = Camera.main;

        shootTrail = GetComponent<LineRenderer>();
        if (shootTrail != null) shootTrail.enabled = false;
    }

    protected override void Update()
    {
        base.Update();

        Debug.DrawRay(shootPoint.position, shootPoint.forward * 100, Color.red);

        if (input.mouse1_Input)
        {
            input.mouse1_Input = false;

            if (!isZooming)
                Zoom();
            else
                Unzoom();
        }

        if (input.mouse0_Input)
        {
            input.mouse0_Input = false;
            Shoot();
        }

        if (input.r_Input)
        {
            input.r_Input = false;
            Reload();
        }
    }

    private void LateUpdate()
    {
        if (isZooming)
        {
            cam.transform.position = shootPoint.position;
            cam.transform.rotation = shootPoint.rotation;
        }
    }

    protected override void Shoot()
    {
        base.Shoot();

        if (isShooting) return;

        if (nowAmmo <= 0)
        {
            Reload();
            return;
        }

        if (reloading) return;

        if (GameManager.Instance.RhythmCheck() > 0)
        {
            Debug.Log("발사 박자 성공");
            anim.SetTrigger("WeaponAttack");
            gameManager.AddCombo();
        }
        else
        {
            Debug.Log("발사 박자 실패");
            gameManager.SetHalfCombo();
            return;
        }

        isShooting = true;

        nowAmmo -= shotAmount;

        if (shootFire != null)
        {
            GameObject fire = Instantiate(shootFire, shootPoint.position, shootPoint.rotation, shootPoint);
            Destroy(fire, 0.1f);
        }

        Vector3 shootDir = shootPoint.forward;

        Ray ray = new Ray(shootPoint.position, shootDir);
        if (Physics.Raycast(ray, out RaycastHit hit_Weak, 100, LayerMask.GetMask("Enemy_Weak")))
        {
            Debug.Log("충돌 대상 : " + hit_Weak.collider.name);

            Debug.Log("약점 명중");
            hit_Weak.collider.GetComponent<EntityWeak>().GetDamage(shotDamage * 2);

            DrawTrail(shootPoint.position, hit_Weak.point);
        }
        else if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Enemy")))
        {
            Debug.Log("충돌 대상 : " + hit.collider.name);

            Debug.Log("명중");
            hit.collider.GetComponent<Entity>().GetDamage(shotDamage);

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
        if (nowAmmo == maxAmmo)
        {
            return;
        }

        reloading = true;

        if (GameManager.Instance.RhythmCheck() > 0)
        {
            Debug.Log("장전 박자 성공");
            reload++;
            gameManager.AddCombo();
        }
        else
        {
            Debug.Log("장전 박자 실패");
            gameManager.SetHalfCombo();
        }

        anim.SetInteger("WeaponReload", reload);
    }

    private void Zoom()
    {
        if (isZooming) return;

        cam = Camera.main;

        originCamParent = cam.transform.parent;
        originCamPos = cam.transform.localPosition;
        originCamRot = cam.transform.localRotation;

        cam.transform.SetParent(shootPoint);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        cam.fieldOfView /= zoomScale;

        scopeUI?.SetActive(true);
        isZooming = true;
    }

    private void Unzoom()
    {
        if (!isZooming) return;

        cam.transform.SetParent(originCamParent);
        cam.transform.localPosition = originCamPos;
        cam.transform.localRotation = originCamRot;
        cam.fieldOfView *= zoomScale;

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

    #region 애니메이션 이벤트
    public void ShootOverEvent() => isShooting = false;

    public void ReloadOverEvent()
    {
        Debug.Log("장전 성공");

        nowAmmo = maxAmmo;

        reloading = false;
        reload = 0;
        anim.SetInteger("WeaponReload", reload);
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
