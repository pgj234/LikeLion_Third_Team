using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UserInterface : MonoBehaviour
{
    #region Default Interface Variables
    [Header("Default Interface Variables")]
    [SerializeField] TextMeshProUGUI ammoLabel;                 // 탄약 레이블
    [SerializeField] RectTransform hp;                          // 체력 이미지
    [SerializeField] float hpMaxRect = 650f;                    // 체력 이미지 최대 크기
    [SerializeField] RectTransform[] dash;                      // 대쉬 이미지
    [SerializeField] float dashMaxRect = 170f;                  // 대쉬 이미지 최대 크기

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
    [SerializeField] Player player;
    #endregion

    #region ItemPickup Interface Variables
    [Space(10)]
    [Header("ItemPickup Interface Variables")]
    [SerializeField] GameObject itemPickupPanel;                // 아이템 획득 패널 오브젝트
    [SerializeField] Transform itemPickupViewport;              // 스크롤 뷰포트 트랜스폼
    [SerializeField] GameObject itemPickupPrefab;               // 아이템 픽업 프리팹
    [SerializeField] List<Sprite> itemIcon;                     // 아이템 아이콘 리스트              / 0 : 검, 1 : 샷건, 2 : 스나, 3 : 권총
    #endregion

    #region Pause Interface Variables
    [Space(10)]
    [Header("Pause Interface Variables")]
    [SerializeField] GameObject pausePanel;                     // 패널 오브젝트
    [SerializeField] Slider bgmSlider;
    [SerializeField] TextMeshProUGUI bgmLabel;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TextMeshProUGUI sfxLabel;
    [SerializeField] Slider mouseSlider;
    [SerializeField] TextMeshProUGUI mouseLabel;

    #endregion

    #region Death Interface Variables
    [Space(10)]
    [Header("Death Interface Variables")]
    [SerializeField] GameObject deathPanel;                     // 죽으면 뜨는 창
    [SerializeField] GameObject resurrectionPanel;              // 부활 창
    [SerializeField] TextMeshProUGUI resurrectionCountLabel;    // 부활 카운트 레이블
    [SerializeField] GameObject gameOverPanel;                  // 게임오버 창
    [SerializeField] int resurrectionCount = 1;                 // 부활 횟수
    [SerializeField] float resurrectionTime = 10f;               // 부활 대기 시간
    Coroutine resurrectionCoroutine; // 부활 카운트 코루틴

    #endregion


    void Start()
    {
        inputManager = InputManager.Instance;                  // InputManager 인스턴스 초기화
        eventManager = EventManager.Instance;                  // EventManager 인스턴스 초기화
        settingManager = UserSettingManager.Instance;          // UserSettingManager 인스턴스 초기화
        if(player == null)
            player = FindFirstObjectByType<Player>();                // Player 인스턴스 초기화 (씬에 Player가 없을 경우)

        for (int i = 0; i < weaponSlot.Length; i++)
        {
            if (i == nowWeaponNum)
                EnableWeaponSlot(weaponSlot[i]);
            else
                DisableWeaponSlot(weaponSlot[i]);
        }

        
        //eventManager.OnPlayerDashUIRefreshAction += Dash;                   // 대쉬 UI 새로고침 이벤트 등록
        eventManager.OnPlayerCurrentBulletUIRefreshAction += UpdateAmmo;    // 플레이어 현재 장전 탄 수 UI 새로고침 이벤트 등록
        //eventManager.OnPlayerMaxBulletUIRefreshAction += (maxBullet) => ammoLabel.text = maxBullet.ToString(); // 플레이어 최대 장전 탄 수 UI 새로고침 이벤트 등록

        eventManager.OnPlayerWeaponUIRefreshAction += ChangeWeaponAnimation;// 플레이어 사용가능 무기 UI 새로고침 이벤트 등록
        eventManager.OnPlayerComboRefreshAction += PlayComboAnimation;      // 콤보 새로고침 이벤트 등록
        eventManager.OnScoreRefreshAction += UpdateScore;                   // 스코어 새로고침 이벤트 등록
        //inputManager.player


        eventManager.OnPlayerDamageAction += UpdateHP;                      // 플레이어 데미지 이벤트 등록
        eventManager.OnPlayerDieAction += ShowDeathPanel;                        // 플레이어 사망 이벤트 등록
        //eventManager.OnPlayerRevivalAction += SelectResurrection;           // 플레이어 부활 이벤트 등록

        foreach (Transform trf in itemPickupViewport)
            Destroy(trf.gameObject);

        for(int i=0;i<player.weaponArray.Length; i++)
        {
            if (player.weaponArray[i].useAble)
                AcquireWeapon(i); // 플레이어가 가진 무기 슬롯 활성화
            else if (weaponSlot[i].activeSelf)
                weaponSlot[i].SetActive(false); // 플레이어가 가진 무기 슬롯 비활성화
        }
    }

    private void OnDisable()
    {
        //eventManager.OnPlayerDashUIRefreshAction -= Dash;                       // 대쉬 UI 새로고침 이벤트 등록해제
        eventManager.OnPlayerCurrentBulletUIRefreshAction -= UpdateAmmo;        // 플레이어 현재 장전 탄 수 UI 새로고침 이벤트 등록해제

        eventManager.OnPlayerWeaponUIRefreshAction -= ChangeWeaponAnimation;    // 플레이어 사용가능 무기 UI 새로고침 이벤트 등록해제
        eventManager.OnPlayerComboRefreshAction -= PlayComboAnimation;          // 콤보 새로고침 이벤트 등록해제

        eventManager.OnPlayerDamageAction -= UpdateHP;                          // 플레이어 데미지 이벤트 등록해제
        eventManager.OnPlayerDieAction -= SelectDie;                            // 플레이어 사망 이벤트 등록해제
        eventManager.OnPlayerRevivalAction -= SelectResurrection;               // 플레이어 부활 이벤트 등록해제
    }

    /// <summary> 테스트용 메서드 </summary>
    private void Update()
    {
        //// 콤보 애니메이션
        //if (Input.GetKeyDown(KeyCode.F12))
        //    PlayComboAnimation(999);

        //// 검 선택
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    ChangeWeaponAnimation(new bool[] { true, false, false, false });
        //// 샷건 선택
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //    ChangeWeaponAnimation(new bool[] { false, true, false, false });
        //// 스나 선택
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //    ChangeWeaponAnimation(new bool[] { false, false, true, false });
        //// 권총 선택
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //    ChangeWeaponAnimation(new bool[] { false, false, false, true });


        //// 검 활성화
        //if (Input.GetKeyDown(KeyCode.F1))
        //    AcquireWeapon(0); // 검 슬롯 활성화
        //// 샷건 활성화
        //else if (Input.GetKeyDown(KeyCode.F2))
        //    AcquireWeapon(1); // 샷건 슬롯 활성화
        //// 스나 활성화
        //else if (Input.GetKeyDown(KeyCode.F3))
        //    AcquireWeapon(2); // 스나 슬롯 활성화
        //// 권총 활성화
        //else if (Input.GetKeyDown(KeyCode.F4))
        //    AcquireWeapon(3); // 권총 슬롯 활성화

        //// 검 비활성화
        //if (Input.GetKeyDown(KeyCode.F5))
        //    weaponSlot[0].SetActive(false); // 검 슬롯 비활성화
        //// 샷건 비활성화
        //if (Input.GetKeyDown(KeyCode.F6))
        //    weaponSlot[1].SetActive(false); // 샷건 슬롯 비활성화
        //// 스나 비활성화
        //if (Input.GetKeyDown(KeyCode.F7))
        //    weaponSlot[2].SetActive(false); // 스나 슬롯 비활성화
        //// 권총 비활성화
        //if (Input.GetKeyDown(KeyCode.F8))
        //    weaponSlot[3].SetActive(false); // 권총 슬롯 비활성화

        //// 아이템 픽업 패널 테스트용
        //if (Input.GetKeyDown(KeyCode.BackQuote))
        //    ShowItemPickUpPanel(Random.Range(0, itemIcon.Count), Random.Range(1, 999)); 

        // Pause 패널 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }

        Dash(player.GetDashGauge());
    }


    #region Player Event State Methods
    void UpdateAmmo(int ammo)
    {
        ammoLabel.text = ammo.ToString(); // 현재 장전 탄 수 레이블 업데이트
    }

    void UpdateHP(int health)
    {
        hp.sizeDelta = new Vector2(hpMaxRect * (health / player.maxHp), hp.sizeDelta.y); // 체력 이미지 크기 업데이트
    }

    void Dash(float stack)
    {
        if(stack < 1f)
        {
            if (dash[1].GetComponent<Image>().enabled == true)
                dash[1].GetComponent<Image>().enabled = false; // 대쉬 이미지 비활성화
            dash[0].sizeDelta = new Vector2(dashMaxRect * stack, dash[0].sizeDelta.y); // 대쉬 이미지 크기 업데이트
        }
        else if(stack < 2f)
        {
            dash[0].sizeDelta = new Vector2(dashMaxRect, dash[0].sizeDelta.y); // 대쉬 이미지 크기 업데이트
            if (dash[1].GetComponent<Image>().enabled == false)
                dash[1].GetComponent<Image>().enabled = true; // 대쉬 이미지 활성화
            dash[1].sizeDelta = new Vector2(dashMaxRect * (stack - 1f), dash[1].sizeDelta.y); // 두번째 대쉬 이미지 크기 업데이트
        }
        else
        {
            dash[0].sizeDelta = new Vector2(dashMaxRect, dash[0].sizeDelta.y); // 대쉬 이미지 크기 업데이트
            dash[1].sizeDelta = new Vector2(dashMaxRect, dash[1].sizeDelta.y); // 두번째 대쉬 이미지 크기 업데이트
        }
    }

    void UpdateScore(int score)
    {
        scoreLabel.text = score.ToString("#,##0"); // 점수 레이블 업데이트
    }
    #endregion

    #region Weapon Related Methods
    public void ChangeWeaponAnimation(bool[] weapons)
    {
        int index = 0;
        for(int i=0;i<weapons.Length; i++)
        {
            if (weapons[i])
            {
                index = i; // 현재 활성화된 무기 번호 업데이트
                EnableWeaponSlot(weaponSlot[i]); // 현재 무기 슬롯 활성화
            }
            else if(nowWeaponNum == i)
            {
                if (weaponSlot[i].activeSelf)
                    DisableWeaponSlot(weaponSlot[i]); // 이전 무기 슬롯 비활성화
                else
                    AcquireWeapon(i);
            }
        }
        nowWeaponNum = index; // 현재 무기 번호 업데이트
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
        disableImg.DOKill();
        disableImg.DOColor(enableStartColor, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            enableImg.enabled = true; // Enable 이미지 활성화
            enableImg.color = enableStartColor; // 투명하게 설정
            enableImg.DOKill();
            enableImg.DOColor(Color.white, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
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
        enableImg.DOKill();
        enableImg.DOColor(enableStartColor, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            disableImg.enabled = true;
            disableImg.color = enableStartColor;
            disableImg.DOKill();
            disableImg.DOColor(disableColor, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //disableRect.DOScale(Vector3.zero, 0.5f); // Disable 이미지 크기 애니메이션
            });
            enableImg.color = new Color(1f, 1f, 1f, 0.5f);
            enableImg.enabled = false;
        });
    }

    public void AcquireWeapon(int weaponNum)
    {
        if (weaponNum < 0 || weaponNum >= weaponSlot.Length)
            return; // 유효하지 않은 무기 번호

        GameObject go = weaponSlot[weaponNum]; // 해당 무기 슬롯 오브젝트 가져오기
        go.SetActive(true);

        Image bg = go.GetComponent<Image>();
        Image enableImg = go.transform.GetChild(0).GetComponent<Image>();
        RectTransform enableRect = enableImg.GetComponent<RectTransform>();
        Image disableImg = go.transform.GetChild(1).GetComponent<Image>();
        RectTransform disableRect = disableImg.GetComponent<RectTransform>();
        Image icon = go.transform.GetChild(2).GetComponent<Image>();

        Color disableColor = Color.white;
        disableColor.a = 100 / 255f;
        Color c = Color.white;
        c.a = 0;
        Color iconColor = Color.white;
        iconColor.a = 130 / 255f;

        bg.enabled = true;
        disableImg.enabled = true;
        enableImg.enabled = false;
        icon.enabled = true;

        disableImg.color = c;
        disableImg.DOKill();
        disableImg.DOColor(disableColor, 0.2f);
        bg.color = c;
        bg.DOKill();
        bg.DOColor(Color.white, 0.2f);
        icon.color = c;
        icon.DOKill();
        icon.DOColor(iconColor, 0.2f);
    }

    #endregion

    public void ShowItemPickUpPanel(int itemNum, int itemAmount)
    {
        GameObject go = Instantiate(itemPickupPrefab, itemPickupViewport);
        Image bg = go.GetComponent<Image>(); // 배경 이미지 컴포넌트 가져오기
        Image icon = go.transform.GetChild(0).GetComponent<Image>(); // 아이콘 이미지 컴포넌트 가져오기
        if (itemIcon.Count > itemNum)
            icon.sprite = itemIcon[itemNum]; // 아이콘 이미지 설정
        TextMeshProUGUI amountText = go.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); // 아이템 수량 텍스트 컴포넌트 가져오기
        amountText.text = itemAmount.ToString(); // 아이템 수량 설정

        Color c = Color.white;
        c.a = 0f; // 초기 투명도 설정

        bg.DOColor(c, 5f);
        icon.DOColor(c, 5f);
        amountText.DOColor(c, 5f);
        Destroy(go, 5f); // 5초 후에 오브젝트 삭제
    }

    #region Pause Panel Methods

    public void BGMChange()
    {
        settingManager.BGM = bgmSlider.value; // BGM 슬라이더 값으로 설정
        bgmLabel.text = $"{Mathf.FloorToInt(bgmSlider.value * 100)} %";
    }
    public void SFXChange()
    {
        settingManager.SFX = sfxSlider.value; // SFX 슬라이더 값으로 설정
        sfxLabel.text = $"{Mathf.FloorToInt(sfxSlider.value * 100)} %";
    }

    public void MouseSensitivityChange()
    {
        settingManager.MouseSensitivity = mouseSlider.value; // 마우스 감도 슬라이더 값으로 설정
        mouseLabel.text = $"{Mathf.FloorToInt(mouseSlider.value * 100)} %";
    }

    public void SceneMove(string sceneName)
    {
        Time.timeScale = 1f; // 게임 시간 재개
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // 씬 이동
    }

    public void TogglePausePanel()
    {
        pausePanel.SetActive(!pausePanel.activeSelf); // Pause 패널의 활성화 상태를 토글
        if (pausePanel.activeSelf)
        {
            eventManager.PauseWindowOpen(true);

            //Time.timeScale = 0f; // 게임 일시 정지
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; // 커서 보이기
            // 슬라이더 값 초기화
            bgmSlider.maxValue = 1f; // BGM 슬라이더 최대값 설정
            bgmSlider.value = settingManager.BGM; // BGM 슬라이더 값 초기화
            bgmLabel.text = $"{Mathf.FloorToInt(bgmSlider.value * 100)} %"; // BGM 레이블 업데이트
            sfxSlider.maxValue = 1f; // SFX 슬라이더 최대값 설정
            sfxSlider.value = settingManager.SFX; // SFX 슬라이더 값 초기화
            sfxLabel.text = $"{Mathf.FloorToInt(sfxSlider.value * 100)} %"; // SFX 레이블 업데이트
            mouseSlider.maxValue = 1f; // 마우스 감도 슬라이더 최대값 설정
            mouseSlider.value = settingManager.MouseSensitivity; // 마우스 감도 슬라이더 값 초기화
            mouseLabel.text = $"{Mathf.FloorToInt(mouseSlider.value * 100)} %"; // 마우스 감도 레이블 업데이트

            //SoundManager.Instance.StopBGM(); // BGM 정지
        }
        else
        {
            eventManager.PauseWindowOpen(false);

            //Time.timeScale = 1f; // 게임 재개
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // 커서 보이기
            //SoundManager.Instance.PlayBGM(BGM.Test_Bgm); // BGM 재생
        }
    }

    #endregion

    #region Death Panel Methods
    void PlayComboAnimation(int combo)
    {
        Color whiteEnd = new Color(1f, 1f, 1f, 0f);             // 콤보 카운트 레이블 애니메이션 종료 색상
        Color redEnd = new Color(1f, 0f, 0f, 0f);               // 콤보 바 애니메이션 종료 색상

        comboBar.DOKill();                                      // 현재 진행 중인 애니메이션 중지
        comboBar.color = Color.white;                           // 콤보 바 색상을 흰색으로 설정
        comboBar.DOColor(redEnd, 10f).SetEase(Ease.OutQuad);
        comboBar.fillAmount = 1f;                              // 콤보 바의 채움 양을 1로 설정 (100% 채움)
        comboBar.DOFillAmount(0f, 10f).OnComplete(() =>
        {
            // combo 0
        });

        comboCountLabel.DOKill();                               // 현재 진행 중인 애니메이션 중지
        comboCountLabel.color = Color.white;                    // 콤보 카운트 레이블 색상을 흰색으로 설정
        comboCountLabel.text = combo.ToString();
        comboCountLabel.DOColor(whiteEnd, 10f).SetEase(Ease.OutQuad);

        comboLabel.DOKill();                                    // 현재 진행 중인 애니메이션 중지
        comboLabel.color = Color.white;                         // 콤보 레이블 색상을 흰색으로 설정
        comboLabel.DOColor(redEnd, 10f).SetEase(Ease.OutQuad);
    }

    public void ShowDeathPanel()
    {
        //deathPanel.SetActive(!deathPanel.activeSelf); // Death 패널의 활성화 상태를 토글
        eventManager.PauseWindowOpen(true); // Pause 패널 열기
        if (resurrectionCount > 0)
        {
            resurrectionPanel.SetActive(true); // 부활 패널 활성화
            gameOverPanel.SetActive(false); // 게임오버 패널 비활성화
            resurrectionCoroutine = StartCoroutine(ResurrectionCounter()); // 부활 카운트 시작
        }
        else
        {
            SelectDie(); // 부활 횟수가 0이면 죽음 선택
        }
    }

    IEnumerator ResurrectionCounter()
    {
        int count = resurrectionCount; // 부활 횟수 초기화
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f); // 1초 대기
        while (count > 0)
        {
            resurrectionCountLabel.text = count.ToString(); // 부활 카운트 레이블 업데이트
            yield return wait; // 1초 대기
            count--; // 카운트 감소
        }

        resurrectionPanel.SetActive(false); // 부활 패널 비활성화
        gameOverPanel.SetActive(true); // 게임오버 패널 활성화
    }

    /// <summary> 부활 </summary>
    public void SelectResurrection()
    {
        StopCoroutine(resurrectionCoroutine);
        resurrectionCount--; // 부활 횟수 감소
        resurrectionPanel.SetActive(false); // 부활 패널 비활성화
        eventManager.PauseWindowOpen(false); // Pause 패널 닫기
        eventManager.PlayerRevivalEvent();
    }

    public void SelectDie()
    {
        eventManager.PauseWindowOpen(true); // Pause 패널 닫기
        if (resurrectionCoroutine != null)
            StopCoroutine(resurrectionCoroutine);
        resurrectionPanel.SetActive(false); // 부활 패널 비활성화
        gameOverPanel.SetActive(true); // 게임오버 패널 활성화
    }
    #endregion

    
}