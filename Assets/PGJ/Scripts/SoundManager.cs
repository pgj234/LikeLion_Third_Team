using UnityEngine;

// BGM 목록 enum
public enum BGM
{
    Bgm,

}

// SFX 목록 enum
public enum SFX
{
    EscapeComplete,

}

public class SoundManager : SingletonBehaviour<SoundManager>
{
    AudioSource bgmAudioSource;
    AudioSource sfxAudioSource;

    public AudioClip[] bgmClip;
    public AudioClip[] sfxClip;



    protected override void Init()
    {
        base.Init();

        bgmAudioSource = transform.Find("BGM").GetComponent<AudioSource>();
        sfxAudioSource = transform.Find("SFX").GetComponent<AudioSource>();
    }



    public void PlayBGM(BGM _bgm)
    {
        bgmAudioSource.clip = bgmClip[(int)_bgm];
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }



    public void PlaySFX(SFX _sfx)
    {
        sfxAudioSource.clip = sfxClip[(int)_sfx];
        sfxAudioSource.Play();
    }
}
