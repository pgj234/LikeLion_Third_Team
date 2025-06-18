using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwordDrawSound : MonoBehaviour
{
    [Header("Draw Sword Sound")]
    [Tooltip("�� ���� �� ����� ȿ����")]
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
