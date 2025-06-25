using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserInterface : MonoBehaviour
{
    #region Default Interface Variables
    [Header("Default Interface Variables")]
    [SerializeField] TextMeshProUGUI ammoLabel;                 // ź�� ���̺�
    [SerializeField] RectTransform hp;                          // ü�� �̹���
    [SerializeField] float hpMaxRect = 650f;

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

    #region Death Interface Variables
    [Space(10)]
    [Header("Death Interface Variables")]
    [SerializeField] GameObject deathPanel;                     // �г� ������Ʈ

    #endregion

    #region ItemPickup Interface Variables
    [Space(10)]
    [Header("ItemPickup Interface Variables")]
    [SerializeField] GameObject itemPickupPanel;                // �г� ������Ʈ

    #endregion

    #region Pause Interface Variables
    [Space(10)]
    [Header("Pause Interface Variables")]
    [SerializeField] GameObject pausePanel;                     // �г� ������Ʈ

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
    }

    private void OnEnable()
    {
        EventManager.Instance.OnPlayerComboRefreshAction += PlayComboAnimation; // �޺� ���ΰ�ħ �̺�Ʈ ���
    }

    private void OnDisable()
    {
        eventManager.OnPlayerComboRefreshAction -= PlayComboAnimation; // �޺� ���ΰ�ħ �̺�Ʈ ���
    }

    /// <summary> �׽�Ʈ�� �޼��� </summary>
    private void Update()
    {
        // �޺� �ִϸ��̼�
        if (Input.GetKeyDown(KeyCode.F12))
            PlayComboAnimation(999);

        // �� ���
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeWeaponAnimation(0);
        // ���� ���
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeWeaponAnimation(1);
        // ���� ���
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeWeaponAnimation(2);
        // ���� ���
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeWeaponAnimation(3);
        

        // �� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F1))
            weaponSlot[0].SetActive(true); // �� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F2))
            weaponSlot[1].SetActive(true); // ���� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F3))
            weaponSlot[2].SetActive(true); // ���� ���� Ȱ��ȭ
        // ���� Ȱ��ȭ
        if (Input.GetKeyDown(KeyCode.F4))
            weaponSlot[3].SetActive(true); // ���� ���� Ȱ��ȭ

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
    }


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
        }
        else
        {
            Time.timeScale = 1f; // ���� �簳
        }
    }

    public void ToggleDeathPanel()
    {
        deathPanel.SetActive(!deathPanel.activeSelf); // Death �г��� Ȱ��ȭ ���¸� ���
        if (deathPanel.activeSelf)
        {
            Time.timeScale = 0f; // ���� �Ͻ� ����
        }
        else
        {
            Time.timeScale = 1f; // ���� �簳
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
        
        EnableWeaponSlot(weaponSlot[weaponNum]); // ���� ���� ���� Ȱ��ȭ
        DisableWeaponSlot(weaponSlot[nowWeaponNum]); // ���� ���� ���� ��Ȱ��ȭ

        nowWeaponNum = weaponNum; // ���� ���� ��ȣ ������Ʈ

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
            enableImg.enabled = true; // Enable �̹��� Ȱ��ȭ
            enableImg.color = enableStartColor; // �����ϰ� ����
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
        enableImg.DOColor(enableStartColor, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            disableImg.enabled = true;
            disableImg.color = enableStartColor;
            disableImg.DOColor(disableColor, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                //disableRect.DOScale(Vector3.zero, 0.5f); // Disable �̹��� ũ�� �ִϸ��̼�
            });
            enableImg.color = new Color(1f, 1f, 1f, 0.5f);
            enableImg.enabled = false;
        });
    }
    #endregion
}