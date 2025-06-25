using UnityEngine;

public class Pistol : WeaponBase
{
    //[Header("입력 키")]
    //[SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
    //[SerializeField] private KeyCode reloadKey = KeyCode.R;
    //[SerializeField] private KeyCode unequipKey = KeyCode.X; // 무기 해제 키

    [Header("장전 설정")]
    [SerializeField] private float reloadStepDuration = 0.6f; // 각 장전 단계 사이의 간격
    // private int reloadStep = 0;                (부모에 currentReloadStepNum 변수 이용)      // 0: 준비 안함, 1~3: 각 장전 단계

    protected override void Awake()
    {
        base.Awake();


    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Reload()
    {
        base.Reload();
    }

    protected override void Shoot()
    {
        base.Shoot();
    }

    protected override void Update()
    {
        base.Update();

        if (input.mouse0_Input)        // 공격
        {
            input.mouse0_Input = false;

            if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
            {
                SoundManager.Instance.PlaySFX(SFX.TestRhythm);
                gameManager.AddCombo();
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                //SoundManager.Instance.PlaySFX(SFX.RhythmFail);
                gameManager.SetHalfCombo();
            }

            gameManager.NotePush();
        }
        else if (input.r_Input)         // 재장전
        {
            input.r_Input = false;

            HandleReloadInput();

            if (1 == gameManager.RhythmCheck() || 2 == gameManager.RhythmCheck())
            {
                SoundManager.Instance.PlaySFX(SFX.TestRhythm);
                gameManager.AddCombo();
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                //SoundManager.Instance.PlaySFX(SFX.RhythmFail);
                gameManager.SetHalfCombo();
            }

            gameManager.NotePush();
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

        anim.SetInteger("WeaponReload_", currentReloadStepNum);

        if (3 == currentReloadStepNum)
        {
            currentReloadStepNum = 0;
            reloading = false;
        }
    }
}
