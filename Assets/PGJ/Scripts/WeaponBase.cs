using DG.Tweening.Core.Easing;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected int weaponNum;                // 무기 번호 (스왑용으로도 사용)
    [SerializeField] protected int nowAmmo;                  // 발사준비된 탄환
    [SerializeField] protected int maxAmmo;                  // 재장전시 탄환 최대치 / -1 무제한
    [SerializeField] protected int shotAmount;               // 발사시 나가는 탄환수
    [SerializeField] protected int nextShotTime;             // 다음 발사까지 딜레이 박자
    [SerializeField] protected float shotDamage;             // 탄 데미지

    [SerializeField] protected Vector2 shotSpreadMin;        // 탄퍼짐 최솟값
    [SerializeField] protected Vector2 shotSpreadMax;        // 탄퍼짐 최댓값

    [SerializeField] protected int reloadStepNum;            // 장전 단계

    protected GameManager gameManager;
    protected InputManager input;
    protected Animator anim;

    protected int currentReloadStepNum;     // 현재 장전 단계

    protected int rhythmTimingNum;          // 0 : 박자 타이밍 X, 1 : 정박 타이밍, 2 : 반박 타이밍

    internal bool reloading = false;

    internal bool shotAble = false;
    internal bool useAble = false;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();

        if (null == anim)
        {
            anim = GetComponentInChildren<Animator>();

            if (null == anim)
            {
                Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    //internal virtual void OnEnable()
    //{

    //}

    protected virtual void Start()
    {
        gameManager = GameManager.Instance;
        input = InputManager.Instance;

        nowAmmo = maxAmmo;
    }

    protected virtual void Update()
    {
        // 음악 시작전이면 리턴
        if (false == gameManager.musicStart)
        {
            if (input.mouse0_Input)
            {
                input.mouse0_Input = false;
            }

            if (input.r_Input)
            {
                input.r_Input = false;
            }

            return;
        }

        rhythmTimingNum = gameManager.RhythmCheck();
    }

    protected virtual void Shoot()         // 발사 메서드
    {

    }

    protected virtual void Reload()        // 재장전 메서드
    {
        reloading = true;
    }

    // 무기 집어넣기
    internal virtual void SwapIn()
    {
        gameObject.SetActive(false);
    }

    internal void SetBoolAnimation(string _animParamName, bool _isTrue)
    {
        anim.SetBool(_animParamName, _isTrue);
    }

    internal void SetTrggierAnimation(string _animParamName)
    {
        anim.SetTrigger(_animParamName);
    }

    internal float GetAnimationTime()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length;
    }

    internal int GetMaxAmmo()
    {
        return maxAmmo;
    }

    internal int GetCurrentAmmo()
    {
        return nowAmmo;
    }
}
