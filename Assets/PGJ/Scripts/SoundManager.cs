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

    public AudioClip[] bgmClipArray;
    public AudioClip[] sfxClipArray;



    protected override void Init()
    {
        base.Init();

        bgmAudioSource = transform.Find("BGM").GetComponent<AudioSource>();
        sfxAudioSource = transform.Find("SFX").GetComponent<AudioSource>();
    }



    public void PlayBGM(BGM _bgm)
    {
        bgmAudioSource.clip = bgmClipArray[(int)_bgm];
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }



    public void PlaySFX(SFX _sfx)
    {
        sfxAudioSource.clip = sfxClipArray[(int)_sfx];
        sfxAudioSource.Play();
    }
}
