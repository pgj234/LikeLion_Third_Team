using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon_ShotGun : WeaponBase
{
    InputManager inputManager; // �Է� �Ŵ��� �ν��Ͻ�
    GameManager gameManager; // ���� �Ŵ��� �ν��Ͻ�
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] Transform shootPoint;
    //Animator animator; // �ִϸ����� ������Ʈ
    [SerializeField] float shotSpread;
    [SerializeField] List<AudioClip> gunSound = new();
    [SerializeField] List<GameObject> hitEffect = new();
    [SerializeField] AudioSource audio;
    Player player;

    Dictionary<string, AnimationClip> animDic = new();

    protected override void Awake()
    {
        base.Awake(); // �θ� Ŭ������ Awake ȣ��
        gameManager = GameManager.Instance; // ���� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
        inputManager = InputManager.Instance; // �Է� �Ŵ��� �ν��Ͻ� �ʱ�ȭ
        //animator = GetComponentInChildren<Animator>(); // �ִϸ����� ������Ʈ �ʱ�ȭ
        if (audio == null)
        {
            audio = GetComponent<AudioSource>(); // ����� �ҽ� �ʱ�ȭ
            if(audio == null)
                audio = gameObject.AddComponent<AudioSource>(); // ����� �ҽ��� ������ �߰�
        }

        player = transform.parent.GetComponentInParent<Player>();

    }

    private void OnEnable()
    {
        if (audio == null)
        {
            audio = GetComponent<AudioSource>(); // ����� �ҽ� �ʱ�ȭ
            if (audio == null)
                audio = gameObject.AddComponent<AudioSource>(); // ����� �ҽ��� ������ �߰�
        }
        audio.PlayOneShot(gunSound[4], 1f);
    }

    private void OnDisable()
    {
        audio.Stop(); // ����� �ҽ� ����
        FindAnyObjectByType<AudioSource>().PlayOneShot(gunSound[4], .5f); // �ٸ� ����� �ҽ����� ���� ���
    }

    protected override void Update()
    {
        //base.Update();
        gameManager = gameManager ?? GameManager.Instance; // ���� �Ŵ��� �ν��Ͻ� �缳��
        rhythmTimingNum = gameManager.RhythmCheck();

        //Debug.Log($"input r : {inputManager.r_Input}");
        //Debug.Log($"mouse 0 : {inputManager.mouse0_Input}");
        //if (inputManager.r_Input)
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            //inputManager.r_Input = false; // ������ �Է� �ʱ�ȭ
        }

        else if(Input.GetMouseButtonDown(0))
        //else if(inputManager.mouse0_Input)
        {
            Shoot();
            //inputManager.mouse0_Input = false; // �߻� �Է� �ʱ�ȭ
        }
    }

    protected override void Reload()
    {
        if (nowAmmo == maxAmmo)
        {
            Debug.Log("�������� �ʿ� �����ϴ�!"); // �������� �ʿ� ������ ���� �α� ���
            return; 
        }
        if(anim.GetBool("Reload") == true)
        {
            Debug.Log("�̹� ������ ���Դϴ�!"); // �̹� ������ ���̸� ���� �α� ���
            return;
        }

        int beat = gameManager.RhythmCheck(); // ���� ���� üũ
        Debug.Log($"Beat: {beat}"); // ���� ���� �α� ���

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

        audio.PlayOneShot(gunSound[beat == 0 ? 2 : 3]);
        anim.SetBool("Reload", true); // �ִϸ����� Ʈ���� ����
        

        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.speed = beat == 0 ? 2 : 1;

        float delay = (animDic["Reload"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // �ִϸ��̼� ������ ���
        StartCoroutine(ActionDelay(() =>
        {
            anim.SetBool("Reload", false);
            anim.speed = 1; // �ִϸ��̼� �ӵ� �ʱ�ȭ
        }, delay)); // ������ �ִϸ��̼� ������
        //base.Reload();
    }

    protected override void Shoot()
    {
        if(anim.GetBool("Reload") == true)
        {
            Debug.Log("������ �߿��� �߻��� �� �����ϴ�!"); // ������ ���̸� ���� �α� ���
            return;
        }
        if (anim.GetBool("Fire") == true)
        {
            Debug.Log("�̹� �߻� ���Դϴ�!"); // �̹� �߻� ���̸� ���� �α� ���
            return;
        }
        if (nowAmmo <= 0)
        {
            Debug.Log("�߻� �غ�� źȯ�� �����ϴ�!"); // �߻� �غ�� źȯ�� ������ ���� �α� ���
            return;            
        }

        int beat = gameManager.RhythmCheck(); // ���� ���� üũ
        Debug.Log($"Beat: {beat}"); // ���� ���� �α� ���

        gameManager.NotePush();

        nowAmmo--;                          // �߻� �غ�� źȯ ����
        //base.Shoot();

        StartCoroutine(Fire(beat));
    }



    IEnumerator Fire(int beat)
    {
        audio.PlayOneShot(gunSound[beat == 0 ? 1 : 0]); // �߻� ���� ���
        anim.SetBool("Fire", true); // �ִϸ����� Ʈ���� ����
        Transform trf = Camera.main.transform;

        //beat == 0 ? gunSound[0] : gunSound[1]

        if (animDic.ContainsKey("Fire") == false)
        {
            AnimationClip clip = GetAnimClip("Fire", false);
            if (clip != null)
                animDic.Add("Fire", clip);
        }

        anim.speed = beat == 0 ? 2f : 1f; // �ִϸ��̼� �ӵ� ����

        Vector3 pos = shootPoint.position;
        WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        for (int i = 0; i < shotAmount; i++)
        {
            Quaternion rot = GetSpreadDirection();
            Vector3 v = (trf.forward + trf.right * Mathf.Tan(rot.x * Mathf.Rad2Deg) + trf.up * Mathf.Tan(rot.y * Mathf.Rad2Deg)).normalized;
            Vector3 dir = rot * trf.forward; // ���� ���� ���
            GameObject bullet = Instantiate(bulletPrefab, pos, Quaternion.LookRotation(v));
            bullet.transform.forward = v;
            Bullet b = bullet.GetComponent<Bullet>();
            if(b != null)
            {
                b.Set(_position: pos, _lotation: Quaternion.LookRotation(v), _direction: dir, _speed: bulletSpeed,
                    _damage: beat == 0 ? 4 : beat == 1 ? 2 : 1, _weapon: this,
                    _hitObj: beat == 0 ? hitEffect[1].GetComponent<HitEffectObj>() : hitEffect[0].GetComponent<HitEffectObj>()//,
                    //_shooter: player
                    );
            }
            //yield return waitForFixedUpdate; // ������ ������
            yield return waitForFixedUpdate;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        float delay = (animDic["Fire"].length - stateInfo.normalizedTime) / Mathf.Max(stateInfo.speed, 0.01f); // �ִϸ��̼� ������ ���
        StartCoroutine(ActionDelay(() =>
        {
            anim.SetBool("Fire", false);
            anim.speed = 1f;
        }, delay)); // ������ �ִϸ��̼� ������
    }

    Quaternion GetSpreadDirection()
    {
        // �⺻ ����
        //Vector3 forward = shootPoint.forward;

        // ���� ����
        //float x = Random.Range(shotSpreadMin.x, shotSpreadMax.x);
        //float y = Random.Range(shotSpreadMin.x, shotSpreadMax.y);
        var x = UnityEngine.Random.Range(-shotSpread, shotSpread);
        var y = UnityEngine.Random.Range(-shotSpread, shotSpread);

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
        var clips = anim.runtimeAnimatorController.animationClips;
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
