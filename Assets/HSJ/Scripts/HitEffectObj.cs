using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class AudioSet
{
    public AudioClip clip;     // 오디오 클립
    public float volume;       // 볼륨
    public bool isLoop;          // 반복 여부

    public AudioSet(AudioClip _clip, float _volume = 1f, bool _isLoop = false)
    {
        clip = _clip;
        volume = _volume;
        isLoop = _isLoop;
    }

    public void Set(AudioSource audio)
    {
        if (audio == null) return;
        audio.clip = clip;
        audio.volume = volume;
        audio.loop = isLoop;
    }
}

public class HitEffectObj : MonoBehaviour
{
    List<GameObject> objs;
    UnityAction hitAction;
    UnityAction callback;
    int PlayEndCount = 0;                       // 플레이가 끝난 오브젝트 횟수
    public AudioSet audioSet;                   // 오디오 설정

    public virtual HitEffectObj ObjSet(UnityAction _callback = null)
    {
        // 콜백 함수는 사용하지 않지만, UnityAction을 받는 생성자
        // 필요시 콜백을 호출할 수 있도록 구현 가능
        //audioSet = _audioSet;
        callback = _callback;
        PlayEndCount = 0;

        return this;
    }

    private void OnEnable()
    {
        objs = new List<GameObject>();
        foreach(Transform child in transform)
        {
            GameObject gameObject = child.gameObject;
            objs.Add(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void Play()
    {
        hitAction?.Invoke();
        int playObjs = 0;
        foreach (GameObject obj in objs)
        {
            obj.SetActive(true);
            //obj.transform.position = transform.position;
            //obj.transform.localRotation = transform.localRotation;

            ParticleSystem particle = obj.GetComponent<ParticleSystem>() ?? null;
            if(particle != null)
            {
                particle.Play();
                playObjs++;
                StartCoroutine(AfterAction(delay: particle.main.duration, action: () =>
                {
                    PlayEndCount++;
                    particle.gameObject.SetActive(false);
                }));
            }
        }

        if(audioSet != null)
        {
            AudioSource audio = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            //audioSet?.Set(audio);
            audio.volume = audioSet.volume;
            audio.loop = audioSet.isLoop;
            if (audio.loop)
                audio.Play();
            else
                audio.PlayOneShot(audioSet.clip);
            

            foreach (Transform child in transform)
            {
                AudioSource childAudio = child.GetComponent<AudioSource>();
                if (childAudio != null)
                {
                    //audioSet.Set(childAudio);
                    childAudio.Play();
                }
            }
        }

        StartCoroutine(DisableEvent(playObjs));
    }

    IEnumerator DisableEvent(int count)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (count > PlayEndCount)
            yield return wait;

        foreach (GameObject obj in objs)
        {
            if(obj.activeSelf)
                obj.SetActive(false);
        }

        callback?.Invoke(); // 모든 이펙트가 끝난 후 콜백 호출
    }

    IEnumerator AfterAction(UnityAction action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
