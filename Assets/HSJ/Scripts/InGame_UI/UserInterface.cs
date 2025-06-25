using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserInterface : MonoBehaviour
{
    #region Default Interface Variables
    [Header("Default Interface Variables")]
    [SerializeField] TextMeshProUGUI ammoLabel;                 // 탄약 레이블
    [SerializeField] RectTransform hp;                          // 체력 이미지
    [SerializeField] float hpMaxRect = 650f;

    [SerializeField] GameObject[] weaponSlot;                   // 무기 슬롯 오브젝트 배열
    [SerializeField] TextMeshProUGUI scoreLabel;                // 점수 레이블
    [Header("Combo")]
    [SerializeField] TextMeshProUGUI comboLabel;                // 콤보 레이블
    [SerializeField] Image comboBar;                            // 콤보 바 이미지
    [SerializeField] TextMeshProUGUI comboCountLabel;           // 콤보 카운트 레이블

    InputManager inputManager;                                  // InputManager 인스턴스
    EventManager eventManager;                                  // EventManager 인스턴스
    UserSettingManager settingManager;                          // UserSettingManager 인스턴스
    GameManager gameManager;                                    // GameManager 인스턴스
    [SerializeField] int nowWeaponNum = 0;

    #endregion

    #region Death Interface Variables
    [Space(10)]
    [Header("Death Interface Variables")]
    [SerializeField] GameObject deathPanel;                     // 패널 오브젝트

    #endregion

    #region ItemPickup Interface Variables
    [Space(10)]
    [Header("ItemPickup Interface Variables")]
    [SerializeField] GameObject itemPickupPanel;                // 패널 오브젝트

    #endregion

    #region Pause Interface Variables
    [Space(10)]
    [Header("Pause Interface Variables")]
    [SerializeField] GameObject pausePanel;                     // 패널 오브젝트

    #endregion


    void Start()
    {
        inputManager = InputManager.Instance;                  // InputManager 인스턴스 초기화
        eventManager = EventManager.Instance;                  // EventManager 인스턴스 초기화
        settingManager = UserSettingManager.Instance;          // UserSettingManager 인스턴스 초기화

        for (int i = 0; i < weaponSlot.Length; i++)
        {
            if (i == nowWeaponNum)
                EnableWeaponSlot(weaponSlot[i]);
            else
                DisableWeaponSlot(weaponSlot[i]);
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerComboRefreshAction += PlayComboAnimation; // 콤보 새로고침 이벤트 등록
    }

    private void OnDisable()
    {
        eventManager.OnPlayerComboRefreshAction -= PlayComboAnimation; // 콤보 새로고침 이벤트 등록
    }

    /// <summary> 테스트용 메서드 </summary>
    private void Update()
    {
        // 콤보 애니메이션
        if (Input.GetKeyDown(KeyCode.F12))
            PlayComboAnimation(999);

        // 검 얻기
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeaponAnimation(0);
        // 샷건 얻기
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeaponAnimation(1);
        // 스나 얻기
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeaponAnimation(2);
        // 권총 얻기
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeWeaponAnimation(3);
        

        // 검 활성화
        if (Input.GetKeyDown(KeyCode.F1))
            weaponSlot[0].SetActive(true); // 검 슬롯 활성화
        // 샷건 활성화
        if (Input.GetKeyDown(KeyCode.F2))
            weaponSlot[1].SetActive(true); // 샷건 슬롯 활성화
        // 스나 활성화
        if (Input.GetKeyDown(KeyCode.F3))
            weaponSlot[2].SetActive(true); // 스나 슬롯 활성화
        // 권총 활성화
        if (Input.GetKeyDown(KeyCode.F4))
            weaponSlot[3].SetActive(true); // 권총 슬롯 활성화

        // 검 비활성화
        if (Input.GetKeyDown(KeyCode.F5))
            weaponSlot[0].SetActive(false); // 검 슬롯 비활성화
        // 샷건 비활성화
        if (Input.GetKeyDown(KeyCode.F6))
            weaponSlot[1].SetActive(false); // 샷건 슬롯 비활성화
        // 스나 비활성화
        if (Input.GetKeyDown(KeyCode.F7))
            weaponSlot[2].SetActive(false); // 스나 슬롯 비활성화
        // 권총 비활성화
        if (Input.GetKeyDown(KeyCode.F8))
            weaponSlot[3].SetActive(false); // 권총 슬롯 비활성화
    }


    void PlayComboAnimation(int combo)
    {
        Color whiteEnd = new Color(1f, 1f, 1f, 0f);             // 콤보 카운트 레이블 애니메이션 종료 색상
        Color redEnd = new Color(1f, 0f, 0f, 0f);               // 콤보 바 애니메이션 종료 색상

        comboBar.DOKill();                                      // 현재 진행 중인 애니메이션 중지
        comboBar.color = Color.white;                           // 콤보 바 색상을 흰색으로 설정
        comboBar.DOColor(redEnd, 5f).SetEase(Ease.OutQuad);
        comboBar.fillAmount = 1f;                              // 콤보 바의 채움 양을 1로 설정 (100% 채움)
        comboBar.DOFillAmount(0f, 5f).OnComplete(() =>
        {
            // combo 0
        });

        comboCountLabel.DOKill();                               // 현재 진행 중인 애니메이션 중지
        comboCountLabel.color = Color.white;                    // 콤보 카운트 레이블 색상을 흰색으로 설정
        comboCountLabel.text = combo.ToString();
        comboCountLabel.DOColor(whiteEnd, 5f).SetEase(Ease.OutQuad);

        comboLabel.DOKill();                                    // 현재 진행 중인 애니메이션 중지
        comboLabel.color = Color.white;                         // 콤보 레이블 색상을 흰색으로 설정
        comboLabel.DOColor(redEnd, 5f).SetEase(Ease.OutQuad);
    }

    public void TogglePausePanel()
    {
        pausePanel.SetActive(!pausePanel.activeSelf); // Pause 패널의 활성화 상태를 토글
        if (pausePanel.activeSelf)
        {
            Time.timeScale = 0f; // 게임 일시 정지
        }
        else
        {
            Time.timeScale = 1f; // 게임 재개
        }
    }

    public void ToggleDeathPanel()
    {
        deathPanel.SetActive(!deathPanel.activeSelf); // Death 패널의 활성화 상태를 토글
        if (deathPanel.activeSelf)
        {
            Time.timeScale = 0f; // 게임 일시 정지
        }
        else
        {
            Time.timeScale = 1f; // 게임 재개
        }
    }

    public void ShowItemPickUpPanel(int itemNum)
    {

    }

    #region Weapon Change Action
    public void ChangeWeaponAnimation(int weaponNum)
    {
        if (nowWeaponNum == weaponNum)
            return;
        
        EnableWeaponSlot(weaponSlot[weaponNum]); // 현재 무기 슬롯 활성화
        DisableWeaponSlot(weaponSlot[nowWeaponNum]); // 이전 무기 슬롯 비활성화

        nowWeaponNum = weaponNum; // 현재 무기 번호 업데이트

    }

    void EnableWeaponSlot(GameObject go)
    {
        Image enableImg = go.transform.GetChild(0).GetComponent<Image>();
        RectTransform enableRect = enableImg.GetComponent<RectTransform>();
        Image disableImg = go.transform.GetChild(1).GetComponent<Image>();
        RectTransform disableRect = disableImg.GetComponent<RectTransform>();

        Color disableColor = Color.white;
        disableColor.a = 100 / 255f;
        Color disableInitColor = Color.white;
        disableInitColor.a = 0.65f;
        Color enableStartColor = Color.white;
        enableStartColor.a = 0.03f;

        enableImg.enabled = false;
        disableImg.enabled = true;

        disableImg.color = disableColor;
        disableImg.DOColor(enableStartColor, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            enableImg.enabled = true; // Enable 이미지 활성화
            enableImg.color = enableStartColor; // 투명하게 설정
            enableImg.DOColor(Color.white, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //enableRect.DOScale(Vector3.one, 0.5f); // Enable 이미지 크기 애니메이션
            });
            disableImg.color = disableColor;
            disableImg.enabled = false; // Disable 이미지 비활성화
        });
    }

    void DisableWeaponSlot(GameObject go)
    {
        Image enableImg = go.transform.GetChild(0).GetComponent<Image>();
        RectTransform enableRect = enableImg.GetComponent<RectTransform>();
        Image disableImg = go.transform.GetChild(1).GetComponent<Image>();
        RectTransform disableRect = disableImg.GetComponent<RectTransform>();

        Color disableColor = Color.white;
        disableColor.a = 100 / 255f;
        Color disableInitColor = Color.white;
        disableInitColor.a = 0.65f;
        Color enableStartColor = Color.white;
        enableStartColor.a = 0.03f; // Enable 이미지 시작 색상 (투명)

        disableImg.enabled = false;
        enableImg.enabled = true;

        enableImg.color = Color.white;
        enableImg.DOColor(enableStartColor, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            disableImg.enabled = true;
            disableImg.color = enableStartColor;
            disableImg.DOColor(disableColor, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //disableRect.DOScale(Vector3.zero, 0.5f); // Disable 이미지 크기 애니메이션
            });
            enableImg.color = new Color(1f, 1f, 1f, 0.5f);
            enableImg.enabled = false;
        });
    }
    #endregion
}