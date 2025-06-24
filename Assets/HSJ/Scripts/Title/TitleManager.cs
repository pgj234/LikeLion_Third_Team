using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] List<GameObject> mainBtns;
    [Header("Setting Panel")]
    [SerializeField] GameObject settingPanel;
    [SerializeField] List<Slider> settingSliders;           // 0 : BGM, 1 : SFX, 2 : Mouse Sensitivity
    [SerializeField] List<TextMeshProUGUI> settingTexts;    // 0 : BGM, 1 : SFX, 2 : Mouse Sensitivity
    [SerializeField] List<TextMeshProUGUI> fpsBtnTexts;     // 60, 120, 165, 240
    [SerializeField] List<Sprite> fpsBtnBgs;                // 0 : disabled, 1 : enabled
    [SerializeField] AudioSource[] soundSource;             // 0 : BGM, 1 : SFX
    [Header("Credit Panel")]
    [SerializeField] GameObject creditPanel;

    #region Main Buttons Event
    public void MainBtnEnterEvent(GameObject gameObject)
    {
        if(creditPanel.activeSelf || settingPanel.activeSelf)
            return; // ���� �г��̳� ũ���� �г��� Ȱ��ȭ�Ǿ� ������ �̺�Ʈ ����

        GameObject parentBtn = FindMainParentBtn(gameObject);
        Image outLine = parentBtn.GetComponent<Image>();
        Image fillImg = parentBtn.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI text = parentBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        outLine.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255));
        fillImg.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(10));
        text.color = Color.black;

        foreach (GameObject obj in mainBtns)
        {
            if (gameObject != obj)
            {
                MainBtnExitEvent(obj);
            }
        }
    }

    public void MainBtnDownEvent(GameObject gameObject)
    {
        if (creditPanel.activeSelf || settingPanel.activeSelf)
            return; // ���� �г��̳� ũ���� �г��� Ȱ��ȭ�Ǿ� ������ �̺�Ʈ ����

        GameObject parentBtn = FindMainParentBtn(gameObject);
        Image outLine = parentBtn.GetComponent<Image>();
        Image fillImg = parentBtn.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI text = parentBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        outLine.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255));
        fillImg.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(128));
        text.color = Color.black;

        foreach (GameObject obj in mainBtns)
        {
            if (gameObject != obj)
            {
                MainBtnExitEvent(obj);
            }
        }
    }

    public void MainBtnUpEvent(GameObject gameObject)
    {
        if (creditPanel.activeSelf || settingPanel.activeSelf)
            return; // ���� �г��̳� ũ���� �г��� Ȱ��ȭ�Ǿ� ������ �̺�Ʈ ����

        GameObject parentBtn = FindMainParentBtn(gameObject);
        Image outLine = parentBtn.GetComponent<Image>();
        Image fillImg = parentBtn.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI text = parentBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        outLine.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255));
        fillImg.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(10));
        text.color = Color.black;

        foreach (GameObject obj in mainBtns)
        {
            if (gameObject != obj)
            {
                MainBtnExitEvent(obj);
            }
        }
    }

    public void MainBtnExitEvent(GameObject gameObject)
    {
        if (creditPanel.activeSelf || settingPanel.activeSelf)
            return; // ���� �г��̳� ũ���� �г��� Ȱ��ȭ�Ǿ� ������ �̺�Ʈ ����

        GameObject parentBtn = FindMainParentBtn(gameObject);
        Image outLine = parentBtn.GetComponent<Image>();
        Image fillImg = parentBtn.transform.Find("Fill").GetComponent<Image>();
        TextMeshProUGUI text = parentBtn.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        outLine.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(128));
        fillImg.color = new Color(ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(255), ColorMaxFloat(0));
        text.color = Color.white;
    }

    GameObject FindMainParentBtn(GameObject obj)
    {
        //while (obj.name != "Options" && obj.transform.parent != null)
        //    obj = obj.transform.parent.gameObject;
        return obj;
    }

    float ColorMaxFloat(int colorValue)
    {
        // 0~255 ������ ���� 0~1 ������ ��ȯ
        return colorValue / 255f;
    }
    #endregion

    private void Start()
    {
        // �ʱ�ȭ �۾�
        settingPanel.SetActive(false);
        creditPanel.SetActive(false);

        foreach (GameObject btn in mainBtns)
        {
            // �� ��ư�� �̺�Ʈ ������ �߰�
            MainBtnExitEvent(btn);
        }

        //UserSettingManager.Instance.RegisterSFXChanged(ChangeSFX, true);
        //UserSettingManager.Instance.RegisterBGMChanged(ChangeBGM, true);
        //UserSettingManager.Instance.RegisterMouseSensitivityChanged(ChangeMouseSensitivity, true);
        soundSource[0].volume = UserSettingManager.Instance.BGM; // BGM ����� �ҽ� ���� ����
    }

    public void SceneMove(int sceneIndex)
    {
        // �� ��ȯ�� ���� SceneManager�� ���
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void ToggleSetting()
    {
        settingPanel.SetActive(!settingPanel.activeSelf);
        if(settingPanel.activeSelf)
        {
            // ���� �г��� Ȱ��ȭ ���¸� ���
            for (int i = 0; i < settingSliders.Count; i++)
            {
                settingSliders[i].value = i switch
                {
                    0 => UserSettingManager.Instance.BGM,
                    1 => UserSettingManager.Instance.SFX,
                    2 => UserSettingManager.Instance.MouseSensitivity,
                    _ => settingSliders[i].value
                };
                settingTexts[i].text = $"{Mathf.FloorToInt(settingSliders[i].value * 100)} %";
            }

            foreach(TextMeshProUGUI text in fpsBtnTexts)
            {
                Image img = text.transform.parent.GetComponent<Image>();
                img.sprite = text.text == UserSettingManager.Instance.FPS.ToString() ? fpsBtnBgs[1] : fpsBtnBgs[0];
                text.color = Color.white; // �ؽ�Ʈ ������ �ʱ�ȭ
            }
        }
        else
        {
            foreach(GameObject go in mainBtns)
            {
                MainBtnExitEvent(go);
            }
        }
    }

    public void ToggleCradit()
    {
        creditPanel.SetActive(!creditPanel.activeSelf);
        if(!creditPanel.activeSelf)
        {
            foreach (GameObject go in mainBtns)
            {
                MainBtnExitEvent(go);
            }
        }
    }

    public void ChangeBGM(Slider slider)
    {
        // BGM ���� ���� ����
        UserSettingManager.Instance.BGM = slider.value;
        settingTexts[0].text = $"{Mathf.FloorToInt(slider.value * 100)} %";
        soundSource[0].volume = slider.value; // BGM ����� �ҽ� ���� ����
    }

    public void ChangeSFX(Slider slider)
    {
        // SFX ���� ���� ����
        UserSettingManager.Instance.SFX = slider.value;
        settingTexts[1].text = $"{Mathf.FloorToInt(slider.value * 100)} %";
        if (soundSource[1].isPlaying)
            soundSource[1].Stop();
        soundSource[1].volume = slider.value; // SFX ����� �ҽ� ���� ����
        soundSource[1].Play(); // SFX ����� �ҽ� ���
    }

    public void ChangeMouseSensitivity(Slider slider)
    {
        // ���콺 ���� ���� ����
        UserSettingManager.Instance.MouseSensitivity = slider.value;
        settingTexts[2].text = $"{Mathf.FloorToInt(slider.value * 100)} %";
    }

    public void ChangeFPS(int value)
    {
        // FPS ���� ���� ����
        UserSettingManager.Instance.FPS = (UserSettingManager.FPSSetting)value;
        foreach(TextMeshProUGUI text in fpsBtnTexts)
        {
            text.color = Color.white; // ��� �ؽ�Ʈ ������ �ʱ�ȭ
        }
    }

    #region FPS Button Events
    public void FPSBtnEnter(GameObject gameObject)
    {
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        text.color = new Color(ColorMaxFloat(50), ColorMaxFloat(50), ColorMaxFloat(50), ColorMaxFloat(255));
    }

    public void FPSBtnDown(GameObject gameObject)
    {
        Image bg = gameObject.GetComponent<Image>();
        bg.sprite = fpsBtnBgs[1]; // Ȱ��ȭ�� ��� �̹����� ����
        foreach(TextMeshProUGUI text in fpsBtnTexts)
        {
            Image btnBG = text.transform.parent.GetComponent<Image>();
            if(btnBG != bg)
                btnBG.sprite = fpsBtnBgs[0]; // ��Ȱ��ȭ�� ��� �̹����� ����
            text.color = Color.white; // ��� �ؽ�Ʈ ������ �ʱ�ȭ
        }
    }

    public void FPSBtnExit(GameObject gameObject)
    {
        TextMeshProUGUI text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        text.color = Color.white;
    }
    #endregion
}
