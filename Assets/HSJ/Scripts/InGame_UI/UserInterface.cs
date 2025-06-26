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
    [SerializeField] TextMeshProUGUI ammoLabel;                 // ź�� ���̺�
    [SerializeField] RectTransform hp;                          // ü�� �̹���
    [SerializeField] float hpMaxRect = 650f;                    // ü�� �̹��� �ִ� ũ��
    [SerializeField] RectTransform dash;                        // �뽬 �̹���
    [SerializeField] float dashMaxRect = 170f;                  // �뽬 �̹��� �ִ� ũ��

    [SerializeField] GameObject[] weaponSlot;                   // ���� ���� ������Ʈ �迭
    [SerializeField] TextMeshProUGUI scoreLabel;                // ���� ���̺�
    [Header("Combo")]
    [SerializeField] TextMeshProUGUI comboLabel;                // �޺� ���̺�
    [SerializeField] Image comboBar;                            // �޺� �� �̹���
    [SerializeField] TextMeshProUGUI comboCountLabel;           // �޺� ī��Ʈ ���̺�

    InputManager inputManager;                                  // InputManager �ν��Ͻ�
    EventManager eventManager;                                  // EventManager �ν��Ͻ�
    UserSettingManager settingManager;                          // UserSettingManager �ν��Ͻ�
    GameManager gameManager;                                    // GameManager �ν��Ͻ�
    [SerializeField] int nowWeaponNum = 0;

    #endregion

    #region ItemPickup Interface Variables
    [Space(10)]
    [Header("ItemPickup Interface Variables")]
    [SerializeField] GameObject itemPickupPanel;                // ������ ȹ�� �г� ������Ʈ
    [SerializeField] Transform itemPickupViewport;              // ��ũ�� ����Ʈ Ʈ������
    [SerializeField] GameObject itemPickupPrefab;               // ������ �Ⱦ� ������
    [SerializeField] List<Sprite> itemIcon;                     // ������ ������ ����Ʈ              / 0 : ��, 1 : ����, 2 : ����, 3 : ����
    #endregion

    #region Pause Interface Variables
    [Space(10)]
    [Header("Pause Interface Variables")]
    [SerializeField] GameObject pausePanel;                     // �г� ������Ʈ
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
    [SerializeField] GameObject deathPanel;                     // ������ �ߴ� â
    [SerializeField] GameObject resurrectionPanel;              // ��Ȱ â
    [SerializeField] TextMeshProUGUI resurrectionCountLabel;    // ��Ȱ ī��Ʈ ���̺�
    [SerializeField] GameObject gameOverPanel;                  // ���ӿ��� â
    [SerializeField] int resurrectionCount = 3;                 // ��Ȱ Ƚ��
    [SerializeField] float resurrectionTime = 10f;               // ��Ȱ ��� �ð�
    Coroutine resurrectionCoroutine; // ��Ȱ ī��Ʈ �ڷ�ƾ

    #endregion


    void Start()
    {
        inputManager = InputManager.Instance;                  // InputManager �ν��Ͻ� �ʱ�ȭ
        eventManager = EventManager.Instance;                  // EventManager �ν��Ͻ� �ʱ�ȭ
        settingManager = UserSettingManager.Instance;          // UserSettingManager �ν��Ͻ� �ʱ�ȭ

        for (int i = 0; i < weaponSlot.Length; i++)
        {
            if (i == nowWeaponNum)
                EnableWeaponSlot(weaponSlot[i]);
            else
                DisableWeaponSlot(weaponSlot[i]);
        }

        
        eventManager.OnPlayerDashUIRefreshAction += Dash;                   // �뽬 UI ���ΰ�ħ �̺�Ʈ ���
        eventManager.OnPlayerCurrentBulletUIRefreshAction += UpdateAmmo;    // �÷��̾� ���� ���� ź �� UI ���ΰ�ħ �̺�Ʈ ���
        //eventManager.OnPlayerMaxBulletUIRefreshAction += (maxBullet) => ammoLabel.text = maxBullet.ToString(); // �÷��̾� �ִ� ���� ź �� UI ���ΰ�ħ �̺�Ʈ ���

        eventManager.OnPlayerWeaponUIRefreshAction += ChangeWeaponAnimation;// �÷��̾� ��밡�� ���� UI ���ΰ�ħ �̺�Ʈ ���
        eventManager.OnPlayerComboRefreshAction += PlayComboAnimation;      // �޺� ���ΰ�ħ �̺�Ʈ ���

        eventManager.OnPlayerDamageAction += UpdateHP;                      // �÷��̾� ������ �̺�Ʈ ���
        eventManager.OnPlayerDieAction += SelectDie;                        // �÷��̾� ��� �̺�Ʈ ���
        eventManager.OnPlayerRevivalAction += SelectResurrection;           // �÷��̾� ��Ȱ �̺�Ʈ ���

        foreach (Transform trf in itemPickupViewport)
            Destroy(trf.gameObject);
    }

    private void OnDisable()
    {
        eventManager.OnPlayerDashUIRefreshAction -= Dash;                       // �뽬 UI ���ΰ�ħ �̺�Ʈ �������
        eventManager.OnPlayerCurrentBulletUIRefreshAction -= UpdateAmmo;        // �÷��̾� ���� ���� ź �� UI ���ΰ�ħ �̺�Ʈ �������

        eventManager.OnPlayerWeaponUIRefreshAction -= ChangeWeaponAnimation;    // �÷��̾� ��밡�� ���� UI ���ΰ�ħ �̺�Ʈ �������
        eventManager.OnPlayerComboRefreshAction -= PlayComboAnimation;          // �޺� ���ΰ�ħ �̺�Ʈ �������

        eventManager.OnPlayerDamageAction -= UpdateHP;                          // �÷��̾� ������ �̺�Ʈ �������
        eventManager.OnPlayerDieAction -= SelectDie;                            // �÷��̾� ��� �̺�Ʈ �������
        eventManager.OnPlayerRevivalAction -= SelectResurrection;               // �÷��̾� ��Ȱ �̺�Ʈ �������
    }

    /// <summary> �׽�Ʈ�� �޼��� </summary>
    private void Update()
    {
        // �޺� �ִϸ��̼�
        if (Input.GetKeyDown(KeyCode.F12))
            PlayComboAnimation(999);

        // �� ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeaponAnimation(new bool[] { true, false, false, false });
        // ���� ����
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeaponAnimation(new bool[] { false, true, false, false });
        // ���� ����
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeaponAnimation(new bool[] { false, false, true, false });
        // ���� ����
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeWeaponAnimation(new bool[] { false, false, false, true });


        // �� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F1))
            AcquireWeapon(0); // �� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        else if (Input.GetKeyDown(KeyCode.F2))
            AcquireWeapon(1); // ���� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        else if (Input.GetKeyDown(KeyCode.F3))
            AcquireWeapon(2); // ���� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        else if (Input.GetKeyDown(KeyCode.F4))
            AcquireWeapon(3); // ���� ���� Ȱ��ȭ

        // �� ��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F5))
            weaponSlot[0].SetActive(false); // �� ���� ��Ȱ��ȭ
        // ���� ��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F6))
            weaponSlot[1].SetActive(false); // ���� ���� ��Ȱ��ȭ
        // ���� ��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F7))
            weaponSlot[2].SetActive(false); // ���� ���� ��Ȱ��ȭ
        // ���� ��Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F8))
            weaponSlot[3].SetActive(false); // ���� ���� ��Ȱ��ȭ

        // ������ �Ⱦ� �г� �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.BackQuote))
            ShowItemPickUpPanel(Random.Range(0, itemIcon.Count), Random.Range(1, 999)); 

        // Pause �г� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePausePanel();
        }
    }


    #region Player State Methods
    void UpdateAmmo(int ammo)
    {
        ammoLabel.text = ammo.ToString(); // ���� ���� ź �� ���̺� ������Ʈ
    }

    void UpdateHP(int health)
    {
        hp.sizeDelta = new Vector2(hpMaxRect * (health / 100f), hp.sizeDelta.y); // ü�� �̹��� ũ�� ������Ʈ
    }

    // TODO �뽬�κ�
    void Dash(int stack)
    {
        
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
                index = i; // ���� Ȱ��ȭ�� ���� ��ȣ ������Ʈ
                EnableWeaponSlot(weaponSlot[i]); // ���� ���� ���� Ȱ��ȭ
            }
            else if(nowWeaponNum == i)
            {
                DisableWeaponSlot(weaponSlot[i]); // ���� ���� ���� ��Ȱ��ȭ
            }
        }
        nowWeaponNum = index; // ���� ���� ��ȣ ������Ʈ
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
        disableImg.DOColor(enableStartColor, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            enableImg.enabled = true; // Enable �̹��� Ȱ��ȭ
            enableImg.color = enableStartColor; // �����ϰ� ����
            enableImg.DOKill();
            enableImg.DOColor(Color.white, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //enableRect.DOScale(Vector3.one, 0.5f); // Enable �̹��� ũ�� �ִϸ��̼�
            });
            disableImg.color = disableColor;
            disableImg.enabled = false; // Disable �̹��� ��Ȱ��ȭ
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
        enableStartColor.a = 0.03f; // Enable �̹��� ���� ���� (����)

        disableImg.enabled = false;
        enableImg.enabled = true;

        enableImg.color = Color.white;
        enableImg.DOKill();
        enableImg.DOColor(enableStartColor, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            disableImg.enabled = true;
            disableImg.color = enableStartColor;
            disableImg.DOKill();
            disableImg.DOColor(disableColor, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //disableRect.DOScale(Vector3.zero, 0.5f); // Disable �̹��� ũ�� �ִϸ��̼�
            });
            enableImg.color = new Color(1f, 1f, 1f, 0.5f);
            enableImg.enabled = false;
        });
    }

    public void AcquireWeapon(int weaponNum)
    {
        if (weaponNum < 0 || weaponNum >= weaponSlot.Length)
            return; // ��ȿ���� ���� ���� ��ȣ

        GameObject go = weaponSlot[weaponNum]; // �ش� ���� ���� ������Ʈ ��������
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
        disableImg.DOColor(disableColor, 0.5f);
        bg.color = c;
        bg.DOKill();
        bg.DOColor(Color.white, 0.5f);
        icon.color = c;
        icon.DOKill();
        icon.DOColor(iconColor, 0.5f);
    }

    #endregion

    public void ShowItemPickUpPanel(int itemNum, int itemAmount)
    {
        GameObject go = Instantiate(itemPickupPrefab, itemPickupViewport);
        Image bg = go.GetComponent<Image>(); // ��� �̹��� ������Ʈ ��������
        Image icon = go.transform.GetChild(0).GetComponent<Image>(); // ������ �̹��� ������Ʈ ��������
        if (itemIcon.Count > itemNum)
            icon.sprite = itemIcon[itemNum]; // ������ �̹��� ����
        TextMeshProUGUI amountText = go.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); // ������ ���� �ؽ�Ʈ ������Ʈ ��������
        amountText.text = itemAmount.ToString(); // ������ ���� ����

        Color c = Color.white;
        c.a = 0f; // �ʱ� ���� ����

        bg.DOColor(c, 5f);
        icon.DOColor(c, 5f);
        amountText.DOColor(c, 5f);
        Destroy(go, 5f); // 5�� �Ŀ� ������Ʈ ����
    }

    #region Pause Panel Methods

    public void BGMChange()
    {
        settingManager.BGM = bgmSlider.value; // BGM �����̴� ������ ����
        bgmLabel.text = $"{Mathf.FloorToInt(bgmSlider.value * 100)} %";
    }
    public void SFXChange()
    {
        settingManager.SFX = sfxSlider.value; // SFX �����̴� ������ ����
        sfxLabel.text = $"{Mathf.FloorToInt(sfxSlider.value * 100)} %";
    }

    public void MouseSensitivityChange()
    {
        settingManager.MouseSensitivity = mouseSlider.value; // ���콺 ���� �����̴� ������ ����
        mouseLabel.text = $"{Mathf.FloorToInt(mouseSlider.value * 100)} %";
    }

    public void SceneMove(string sceneName)
    {
        Time.timeScale = 1f; // ���� �ð� �簳
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName); // �� �̵�
    }
    #endregion

    #region Death Event Methods
    void PlayComboAnimation(int combo)
    {
        Color whiteEnd = new Color(1f, 1f, 1f, 0f);             // �޺� ī��Ʈ ���̺� �ִϸ��̼� ���� ����
        Color redEnd = new Color(1f, 0f, 0f, 0f);               // �޺� �� �ִϸ��̼� ���� ����

        comboBar.DOKill();                                      // ���� ���� ���� �ִϸ��̼� ����
        comboBar.color = Color.white;                           // �޺� �� ������ ������� ����
        comboBar.DOColor(redEnd, 5f).SetEase(Ease.OutQuad);
        comboBar.fillAmount = 1f;                              // �޺� ���� ä�� ���� 1�� ���� (100% ä��)
        comboBar.DOFillAmount(0f, 5f).OnComplete(() =>
        {
            // combo 0
        });

        comboCountLabel.DOKill();                               // ���� ���� ���� �ִϸ��̼� ����
        comboCountLabel.color = Color.white;                    // �޺� ī��Ʈ ���̺� ������ ������� ����
        comboCountLabel.text = combo.ToString();
        comboCountLabel.DOColor(whiteEnd, 5f).SetEase(Ease.OutQuad);

        comboLabel.DOKill();                                    // ���� ���� ���� �ִϸ��̼� ����
        comboLabel.color = Color.white;                         // �޺� ���̺� ������ ������� ����
        comboLabel.DOColor(redEnd, 5f).SetEase(Ease.OutQuad);
    }

    public void TogglePausePanel()
    {
        pausePanel.SetActive(!pausePanel.activeSelf); // Pause �г��� Ȱ��ȭ ���¸� ���
        if (pausePanel.activeSelf)
        {
            Time.timeScale = 0f; // ���� �Ͻ� ����
            inputManager.cursorInputForLook = false; // ���콺 Ŀ�� ��� ����
            inputManager.cursorLocked = false; // Ŀ�� ��� ����
            // �����̴� �� �ʱ�ȭ
            bgmSlider.value = settingManager.BGM; // BGM �����̴� �� �ʱ�ȭ
            bgmLabel.text = $"{Mathf.FloorToInt(bgmSlider.value * 100)} %"; // BGM ���̺� ������Ʈ
            sfxSlider.value = settingManager.SFX; // SFX �����̴� �� �ʱ�ȭ
            sfxLabel.text = $"{Mathf.FloorToInt(sfxSlider.value * 100)} %"; // SFX ���̺� ������Ʈ
            mouseSlider.value = settingManager.MouseSensitivity; // ���콺 ���� �����̴� �� �ʱ�ȭ
            mouseLabel.text = $"{Mathf.FloorToInt(mouseSlider.value * 100)} %"; // ���콺 ���� ���̺� ������Ʈ

            SoundManager.Instance.StopBGM(); // BGM ����
        }
        else
        {
            Time.timeScale = 1f; // ���� �簳
            inputManager.cursorInputForLook = true; // ���콺 Ŀ�� ���
            inputManager.cursorLocked = true; // Ŀ�� ���
            SoundManager.Instance.PlayBGM(BGM.Test_Bgm); // BGM ���
        }
    }

    public void ShowDeathPanel(int resurrectionCount)
    {
        //deathPanel.SetActive(!deathPanel.activeSelf); // Death �г��� Ȱ��ȭ ���¸� ���

        if (resurrectionCount > 0)
        {
            Time.timeScale = 0f; // ���� �Ͻ� ����
            resurrectionPanel.SetActive(true); // ��Ȱ �г� Ȱ��ȭ
            gameOverPanel.SetActive(false); // ���ӿ��� �г� ��Ȱ��ȭ
            resurrectionCoroutine = StartCoroutine(ResurrectionCounter()); // ��Ȱ ī��Ʈ ����
        }
        else
        {
            Time.timeScale = 0f; // ���� �簳
            gameOverPanel.SetActive(true); // ���ӿ��� �г� Ȱ��ȭ
            resurrectionPanel.SetActive(false); // ��Ȱ �г� ��Ȱ��ȭ
        }
    }

    IEnumerator ResurrectionCounter()
    {
        int count = resurrectionCount; // ��Ȱ Ƚ�� �ʱ�ȭ
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f); // 1�� ���
        while (count > 0)
        {
            resurrectionCountLabel.text = count.ToString(); // ��Ȱ ī��Ʈ ���̺� ������Ʈ
            yield return wait; // 1�� ���
            count--; // ī��Ʈ ����
        }

        resurrectionPanel.SetActive(false); // ��Ȱ �г� ��Ȱ��ȭ
        gameOverPanel.SetActive(true); // ���ӿ��� �г� Ȱ��ȭ
    }

    /// <summary> ��Ȱ </summary>
    public void SelectResurrection()
    {
        StopCoroutine(resurrectionCoroutine);
        resurrectionPanel.SetActive(false); // ��Ȱ �г� ��Ȱ��ȭ
        Time.timeScale = 1f; // ���� �ð� �簳
    }

    public void SelectDie()
    {
        if (resurrectionCoroutine != null)
            StopCoroutine(resurrectionCoroutine);
        resurrectionPanel.SetActive(false); // ��Ȱ �г� ��Ȱ��ȭ
        gameOverPanel.SetActive(true); // ���ӿ��� �г� Ȱ��ȭ
        Time.timeScale = 0f;
    }
    #endregion

    
}