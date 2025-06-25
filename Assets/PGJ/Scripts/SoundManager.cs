using UnityEngine;

// BGM 목록 enum
public enum BGM
{
    Test_Bgm,

}

// SFX 목록 enum
public enum SFX
{
    RhythmFailShot,
    SwordDraw,
    SniperShoot,
    WeaponGet,
    PistolShot,
    PistolCocked,
    PistolSlide,
    PistolEmpty
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

    internal void SetPlayScheduled(double dspStartTime)
    {
        bgmAudioSource.PlayScheduled(dspStartTime);
    }


    public void PlayBGM(BGM _bgm)
    {
        bgmAudioSource.clip = bgmClipArray[(int)_bgm];
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }



    public void PlaySFX(SFX _sfx)
    {
        sfxAudioSource.PlayOneShot(sfxClipArray[(int)_sfx]);
    }
}
