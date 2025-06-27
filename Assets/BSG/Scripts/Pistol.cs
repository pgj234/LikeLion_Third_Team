using Unity.VisualScripting;
using UnityEngine;

public class Pistol : WeaponBase
{
    //[Header("입력 키")]
    //[SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
    //[SerializeField] private KeyCode reloadKey = KeyCode.R;
    //[SerializeField] private KeyCode unequipKey = KeyCode.X; // 무기 해제 키

    //[Header("장전 설정")]
    //[SerializeField] private float reloadStepDuration = 0.6f; // 각 장전 단계 사이의 간격
    // private int reloadStep = 0;                (부모에 currentReloadStepNum 변수 이용)      // 0: 준비 안함, 1~3: 각 장전 단계

    Transform cameraTr;

    protected override void Awake()
    {
        base.Awake();

        cameraTr = Camera.main.transform;
    }

    protected override void Start()
    {
        base.Start();
    }

    //void OnEnable()
    //{
    //    anim.SetTrigger("WeaponPull");
    //}

    protected override void Reload()
    {
        // 탄 꽉참
        if (nowAmmo == maxAmmo)
        {
            return;
        }

        HandleReloadInput();

        gameManager.NotePush();
    }

    protected override void Shoot()
    {
        if (reloading)
        {
            return;
        }

        // 탄 없음
        if (nowAmmo < 1)
        {
            SoundManager.Instance.PlaySFX(SFX.PistolEmpty);
            return;
        }

        anim.Play("Pistol_fire", -1, 0);
        shotEffect.SetActive(false);
        shotEffect.SetActive(true);
        //anim.SetTrigger("WeaponAttack");
        nowAmmo--;
        EventManager.Instance.PlayerCurrentBulletUIRefresh(nowAmmo);

        if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
        {
            SoundManager.Instance.PlaySFX(SFX.PistolShot);
            gameManager.AddCombo();
        }
        else
        {
            Debug.Log("박자 타이밍 실패...");
            SoundManager.Instance.PlaySFX(SFX.RhythmFailShot);
            gameManager.SetHalfCombo();
        }

        Hit();

        gameManager.NotePush();
    }

    public void ShotEffectOff()
    {
        shotEffect.SetActive(false);
    }

    void Hit()
    {
        RaycastHit[] results = new RaycastHit[1]; // 미리 배열 생성
        int hitCount = Physics.SphereCastNonAlloc(cameraTr.position, 0.1f, cameraTr.forward, results, range, LayerMask.GetMask("Ground", "Enemy"));
        
        if (0 < hitCount)
        {
            GameObject go = Instantiate(hitEffectPrefab, results[0].point, Quaternion.LookRotation(results[0].normal) * Quaternion.Euler(90, 0, 0));

            if (results[0].transform.CompareTag("Enemy"))
            {
                if (results[0].transform.TryGetComponent(out Entity enemy))
                {
                    enemy.GetDamage(GetTotalDamage());
                }
            }
            else if (results[0].transform.CompareTag("Enemy_Weak"))
            {
                if (results[0].transform.TryGetComponent(out EntityWeak enemyWeak))
                {
                    enemyWeak.GetDamage(GetTotalDamage() * 2);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 origin = cameraTr.position;
        Vector3 dir = cameraTr.forward;
        
        // 충돌 궤적 시각화
        Gizmos.DrawLine(origin, origin + dir * range);
        DrawWireSphere(origin, 0.1f); // 시작점
        DrawWireSphere(origin + dir * range, 0.1f); // 끝점

        // 중간을 따라 원 그려서 "터널"처럼 시각화 가능
        int segments = 10;
        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pos = origin + dir * range * t;
            DrawWireSphere(pos, 0.1f);
        }
    }

    // 구를 그리는 헬퍼
    private void DrawWireSphere(Vector3 position, float radius)
    {
        Gizmos.DrawWireSphere(position, radius);
    }

    protected override void Update()
    {
        base.Update();

        if (input.mouse0_Input)        // 공격
        {
            input.mouse0_Input = false;

            Shoot();
        }
        else if (input.r_Input)         // 재장전
        {
            input.r_Input = false;

            Reload();
        }
    }

    void HandleReloadInput()
    {
        if (!reloading)
        {
            reloading = true;
            currentReloadStepNum = 1;
        }
        else if (currentReloadStepNum < 3)
        {
            currentReloadStepNum++;
        }

        if (3 > currentReloadStepNum)
        {
            if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
            {
                gameManager.AddCombo();
                SoundManager.Instance.PlaySFX(SFX.PistolCocked);
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                gameManager.SetHalfCombo();
                SoundManager.Instance.PlaySFX(SFX.RhythmFailShot);
            }
        }

        anim.SetInteger("WeaponReload_", currentReloadStepNum);

        if (3 == currentReloadStepNum)
        {
            if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
            {
                gameManager.AddCombo();
                SoundManager.Instance.PlaySFX(SFX.PistolSlide);
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                gameManager.SetHalfCombo();
                SoundManager.Instance.PlaySFX(SFX.RhythmFailShot);
            }

            nowAmmo = maxAmmo;
            EventManager.Instance.PlayerCurrentBulletUIRefresh(nowAmmo);
            currentReloadStepNum = 0;
            reloading = false;
        }
    }
}
