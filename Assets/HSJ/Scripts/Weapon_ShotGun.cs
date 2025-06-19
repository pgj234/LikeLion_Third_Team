using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon_ShotGun : WeaponBase
{
    GameManager gameManager; // ���� �Ŵ��� �ν��Ͻ�
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    Animator animator; // �ִϸ����� ������Ʈ
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();

    private void Start()
    {
        gameManager = GameManager.Instance; // ���� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ �ʱ�ȭ
    }

    protected override void Update()
    {
        //base.Update();
        rhythmTimingNum = gameManager.RhythmCheck();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        else if(Input.GetMouseButtonDown(0))
            Shoot();
    }

    protected override void Reload()
    {
        if (nowAmmo >= maxAmmo)
        {
            Debug.LogError("�������� �ʿ� �����ϴ�!"); // �������� �ʿ� ������ ���� �α� ���
            return; 
        }
        if (gameManager.RhythmCheck() == 0)
        {
            Debug.LogError("���� Ÿ�̹��� �ƴմϴ�!"); // ���� Ÿ�̹��� �ƴϸ� ���� �α� ���
            return; 
        }
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("�̹� ������ ���Դϴ�!"); // �̹� ������ ���̸� ���� �α� ���
            return;
        }

        //if (currentReloadStepNum >= reloadStepNum)
        //{
        //    return; // ���� ���� �ܰ谡 �ִ� �ܰ迡 ���������� ����
        //}
        gameManager.NotePush(); // ��Ʈ Ǫ�� ȣ��
        nowAmmo += 1; // �߻� �غ�� źȯ ����
        animator.SetBool("Reload", true); // �ִϸ����� Ʈ���� ����
        StartCoroutine(ActionDelay(() => animator.SetBool("Reload", false), 0.5f)); // ������ �ִϸ��̼� ������
        //base.Reload();
    }

    protected override void Shoot()
    {
        //TODO 1. RhythmCheck() 0 ���� ��� ��
        //TODO 2. base.Shoot() ���� return�� �Ʒ� �ڵ尡 �����
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("������ �߿��� �߻��� �� �����ϴ�!"); // ������ ���̸� ���� �α� ���
            return;
        }   

        Debug.Log($"RhythmCheck : {gameManager.RhythmCheck()}");
        //Debug.Log(string.Format("rhythmTimingNum : {0}", rhythmTimingNum));
        //if (Time.time < nextShotTime) return; // ������ �ð� üũ
        if (nowAmmo <= 0)
        {
            Debug.LogError("�߻� �غ�� źȯ�� �����ϴ�!"); // �߻� �غ�� źȯ�� ������ ���� �α� ���
            return;            
        }

        if(animator.GetBool("Fire") == true)
        {
            Debug.LogError("�̹� �߻� ���Դϴ�!"); // �̹� �߻� ���̸� ���� �α� ���
            return;
        }

        gameManager.NotePush();

        if (rhythmTimingNum == 0)
        {
            Debug.LogError("���� Ÿ�̹��� �ƴմϴ�!"); // ���� Ÿ�̹��� �ƴϸ� ���� �α� ���
            return;   // ���� Ÿ�̹��� �ƴϸ� ����
        }

        nowAmmo--;                          // �߻� �غ�� źȯ ����
        //base.Shoot();

        StartCoroutine(Fire());
    }



    IEnumerator Fire()
    {

        animator.SetBool("Fire", true); // �ִϸ����� Ʈ���� ����
        Transform trf = Camera.main.transform;
        

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * Camera.main.transform.forward; // ���� ���� ���
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(v));
            bullet.transform.localScale = bulletScale; // ũ�� ����
            bullet.GetComponent<Rigidbody>().linearVelocity = v * bulletSpeed;
            //yield return waitForFixedUpdate; // ������ ������
            yield return null;
        }

        animator.SetBool("Fire", false); // �߻� �ִϸ��̼� ����
    }

    Quaternion GetSpreadDirection()
    {
        // �⺻ ����
        //Vector3 forward = shootPoint.forward;

        // ���� ����
        //float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        //float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);
        var x = Random.Range(-shotSpread, shotSpread);
        var y = Random.Range(-shotSpread, shotSpread);

        // �����̼��� ����� ȸ�� ����
        return Quaternion.Euler(y, x, 0f);
        //Quaternion spreadRotation = Quaternion.Euler(y, x, 0);
        //return spreadRotation * forward;
    }

    IEnumerator ActionDelay(UnityAction action, float delayTime)
    {
        yield return new WaitForSeconds(delayTime); // ������ �ð� ���
        action?.Invoke(); // �׼� ȣ��
    }

    public void ObjectHit()
    {

    }
}
