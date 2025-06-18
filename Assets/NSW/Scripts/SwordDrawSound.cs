using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwordDrawSound : MonoBehaviour
{
    [Header("Draw Sword Sound")]
    [Tooltip("검 꺼낼 때 재생할 효과음")]
    [SerializeField] private AudioClip drawClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = drawClip;
    }

    public void PlayDrawSound()
    {
        if (drawClip == null) return;
        _audioSource.PlayOneShot(drawClip);
    }
}
