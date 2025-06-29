using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UserSettingManager : SingletonBehaviour<UserSettingManager>
{
    #region Setting Values
    [SerializeField] float sfx = 1f;
    UnityAction<float> sfxChangedAction;
    public float SFX
    {
        get { return sfx; }
        set
        {
            sfx = value;
            sfxChangedAction?.Invoke(value);
            PlayerPrefs.SetFloat("Setting.SFX", sfx);
        }
    }


    [SerializeField] float bgm = 1f;
    UnityAction<float> bgmChangedAction;
    public float BGM
    {
        get { return bgm; }
        set
        {
            bgm = value;
            bgmChangedAction?.Invoke(value);
            PlayerPrefs.SetFloat("Setting.BGM", sfx);
        }
    }

    [SerializeField] float mouseSensitivity = 1f;
    UnityAction<float> mouseSensitivityChangedAction;
    public float MouseSensitivity
    {
        get { return mouseSensitivity; }
        set
        {
            mouseSensitivity = value;
            mouseSensitivityChangedAction?.Invoke(value);
            PlayerPrefs.SetFloat("Setting.MouseSpeed", sfx);
        }
    }

    public enum FPSSetting { Low = 60, Medium = 120, High = 165, Ultra = 240 }
    [SerializeField] FPSSetting fps = FPSSetting.Low;
    public FPSSetting FPS
    {
        get { return fps; }
        set
        {
            fps = value;
            PlayerPrefs.SetInt("Setting.FrameRate", (int)value);
            Application.targetFrameRate = (int)value;
        }
    }
    #endregion

    List<UnityAction> unityActions = new List<UnityAction>();

    private void Awake()
    {
        Init();

        // √ ±‚∞™ ∑ŒµÂ
        BGM = PlayerPrefs.GetFloat("Setting.BGM", 0.5f);
        SFX = PlayerPrefs.GetFloat("Setting.SFX", 0.5f);
        MouseSensitivity = PlayerPrefs.GetFloat("Setting.MouseSpeed", 1f);
        FPS = (FPSSetting)PlayerPrefs.GetInt("Setting.FrameRate", (int)FPSSetting.Low);
    }

    private void OnEnable()
    {
        Application.targetFrameRate = (int)FPS;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        foreach(var action in unityActions)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }
        unityActions.Clear();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region EventRegister
    public void RegisterSFXChanged(UnityAction<float> action, bool isInvoke = false)
    {
        sfxChangedAction += action;
        unityActions.Add(() => sfxChangedAction -= action);
        if(isInvoke)
            action.Invoke(SFX);
        
    }

    public void UnregisterSFXChanged(UnityAction<float> action, bool isInvoke = false)
    {
        sfxChangedAction -= action;
        unityActions.Add(() => sfxChangedAction -= action);
        if (isInvoke)
            action.Invoke(SFX);
    }

    public void RegisterBGMChanged(UnityAction<float> action, bool isInvoke = false)
    {
        bgmChangedAction += action;
        unityActions.Add(() => bgmChangedAction -= action);
        if (isInvoke)
            action.Invoke(BGM);
    }

    public void UnRegisterBGMChanged(UnityAction<float> action, bool isInvoke = false)
    {
        bgmChangedAction -= action;
        unityActions.Add(() => bgmChangedAction -= action);
        if (isInvoke)
            action.Invoke(BGM);
    }

    public void RegisterMouseSensitivityChanged(UnityAction<float> action, bool isInvoke = false)
    {
        mouseSensitivityChangedAction += action;
        unityActions.Add(() => mouseSensitivityChangedAction -= action);
        if (isInvoke)
            action.Invoke(MouseSensitivity);
    }

    public void UnregisterMouseSensitivityChanged(UnityAction<float> action, bool isInvoke = false)
    {
        mouseSensitivityChangedAction -= action;
        unityActions.Add(() => mouseSensitivityChangedAction -= action);
        if (isInvoke)
            action.Invoke(MouseSensitivity);
    }
    #endregion

    // æ¿¿Ã πŸ≤Ô »ƒ »£√‚µ 
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
}
