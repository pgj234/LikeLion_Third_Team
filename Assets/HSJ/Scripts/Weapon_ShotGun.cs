using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon_ShotGun : WeaponBase
{
    InputManager inputManager; // �Է� �Ŵ��� �ν��Ͻ�
    GameManager gameManager; // ���� �Ŵ��� �ν��Ͻ�
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletScale = Vector3.one;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    Animator animator; // �ִϸ����� ������Ʈ
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();
    [SerializeField] List<GameObject> hitEffect = new();

    Dictionary<string, AnimationClip> animDic = new();

    private void Start()
    {
        gameManager = GameManager.Instance; // ���� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
        inputManager = InputManager.Instance; // �Է� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
        animator = GetComponentInChildren<Animator>(); // �ִϸ����� ������Ʈ �ʱ�ȭ
    }

    protected override void Update()
    {
        //base.Update();
        rhythmTimingNum = gameManager.RhythmCheck();

        if (inputManager.r_Input)
        {
            Reload();
            inputManager.r_Input = false; // ������ �Է� �ʱ�ȭ
        }

        else if(inputManager.mouse0_Input)
        {
            Shoot();
            inputManager.mouse0_Input = false; // �߻� �Է� �ʱ�ȭ
        }
    }

    protected override void Reload()
    {
        if (nowAmmo >= maxAmmo)
        {
            Debug.LogError("�������� �ʿ� �����ϴ�!"); // �������� �ʿ� ������ ���� �α� ���
            return; 
        }
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("�̹� ������ ���Դϴ�!"); // �̹� ������ ���̸� ���� �α� ���
            return;
        }

        int beat = gameManager.RhythmCheck(); // ���� ���� üũ

        //if (currentReloadStepNum >= reloadStepNum)
        //{
        //    return; // ���� ���� �ܰ谡 �ִ� �ܰ迡 ���������� ����
        //}

        gameManager.NotePush(); // ��Ʈ Ǫ�� ȣ��
        nowAmmo += 1; // �߻� �غ�� źȯ ����

        if(animDic.ContainsKey("Reload") == false)
        {
            AnimationClip clip = GetAnimClip("Reload", false);
            if (clip != null)
                animDic.Add("Reload", clip);
            
        }

        animator.SetBool("Reload", true); // �ִϸ����� Ʈ���� ����
        

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        animator.speed = beat == 0 ? 2 : 1;

        float delay = (animDic["Reload"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // �ִϸ��̼� ������ ���
        StartCoroutine(ActionDelay(() =>
        {
            animator.SetBool("Reload", false);
            animator.speed = 1; // �ִϸ��̼� �ӵ� �ʱ�ȭ
        }, delay)); // ������ �ִϸ��̼� ������
        //base.Reload();
    }

    protected override void Shoot()
    {
        if(animator.GetBool("Reload") == true)
        {
            Debug.LogError("������ �߿��� �߻��� �� �����ϴ�!"); // ������ ���̸� ���� �α� ���
            return;
        }
        if (animator.GetBool("Fire") == true)
        {
            Debug.LogError("�̹� �߻� ���Դϴ�!"); // �̹� �߻� ���̸� ���� �α� ���
            return;
        }
        if (nowAmmo <= 0)
        {
            Debug.LogError("�߻� �غ�� źȯ�� �����ϴ�!"); // �߻� �غ�� źȯ�� ������ ���� �α� ���
            return;            
        }

        int beat = gameManager.RhythmCheck(); // ���� ���� üũ

        gameManager.NotePush();

        nowAmmo--;                          // �߻� �غ�� źȯ ����
        //base.Shoot();

        StartCoroutine(Fire(beat));
    }



    IEnumerator Fire(int beat)
    {

        animator.SetBool("Fire", true); // �ִϸ����� Ʈ���� ����
        Transform trf = Camera.main.transform;

        //beat == 0 ? gunSound[0] : gunSound[1]

        if (animDic.ContainsKey("Fire") == false)
        {
            AnimationClip clip = GetAnimClip("Fire", false);
            if (clip != null)
                animDic.Add("Fire", clip);
        }

        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * Camera.main.transform.forward; // ���� ���� ���
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.LookRotation(v));
            bullet.transform.localScale = bulletScale; // ũ�� ����
            bullet.GetComponent<Rigidbody>().linearVelocity = v * bulletSpeed;
            Bullet b = bullet.GetComponent<Bullet>();
            if(b != null)
            {
                b.Set(shootPoint.position, Quaternion.LookRotation(v), dir, bulletSpeed,
                    bulletSpeed, beat == 0 ? 4 : beat == 1 ? 2 : 1, this,
                    _hitObj: beat == 0 ? hitEffect[0].GetComponent<HitEffectObj>() : hitEffect[1].GetComponent<HitEffectObj>());
            }
            //yield return waitForFixedUpdate; // ������ ������
            yield return waitForFixedUpdate;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        float delay = (animDic["Fire"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // �ִϸ��̼� ������ ���
        StartCoroutine(ActionDelay(() => animator.SetBool("Fire", false), delay)); // ������ �ִϸ��̼� ������
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

    private AnimationClip GetAnimClip(string name, bool isMach = true)
    {
        var clips = animator.runtimeAnimatorController.animationClips;
        foreach (var clip in clips)
        {
            if (isMach && clip.name == name)
                return clip;
            if(!isMach && clip.name.Contains(name))
                return clip;
        }
        return null;
    }
}
