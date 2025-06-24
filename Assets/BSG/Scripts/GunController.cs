using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private Animator animator;

    [Header("입력 키")]
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private KeyCode unequipKey = KeyCode.X; // 무기 해제 키

    [Header("장전 설정")]
    [SerializeField] private float reloadStepDuration = 0.6f; // 각 장전 단계 사이의 간격
    private int reloadStep = 0; // 0: 준비 안함, 1~3: 각 장전 단계
    private bool isReloading = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && !isReloading)
        {
            animator.SetTrigger("WeaponAttack");
        }

        if (Input.GetKeyDown(unequipKey) && !isReloading)
        {
            animator.SetTrigger("WeaponPut");
        }

        if (Input.GetKeyDown(reloadKey))
        {
            HandleReloadInput();
        }
    }

    void HandleReloadInput()
    {

        if (!isReloading)
        {
            isReloading = true;
            reloadStep = 1;
        }
        else if (reloadStep < 3)
        {
            reloadStep++;
        }
        else
        {
            reloadStep = 0;
            isReloading = false;
        }

        animator.SetInteger("WeaponReload_", reloadStep);
    }

    // private System.Collections.IEnumerator ReloadSequence()
    // {
    //     isReloading = true;

    //     animator.SetInteger("WeaponReload_", 1); // 탄알집 제거
    //     yield return new WaitForSeconds(reloadStepDuration);

    //     animator.SetInteger("WeaponReload_", 2); // 탄알집 결합
    //     yield return new WaitForSeconds(reloadStepDuration);

    //     animator.SetInteger("WeaponReload_", 3); // 슬라이드 조작
    //     yield return new WaitForSeconds(reloadStepDuration);

    //     animator.SetInteger("WeaponReload_", 0); // 상태 초기화
    //     isReloading = false;
    // }
}
