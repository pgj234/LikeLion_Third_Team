using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class AudioSet
{
    public AudioClip clip;     // ����� Ŭ��
    public float volume;       // ����
    public bool isLoop;          // �ݺ� ����

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
    int PlayEndCount = 0;                       // �÷��̰� ���� ������Ʈ Ƚ��
    public AudioSet audioSet;                   // ����� ����

    public virtual HitEffectObj ObjSet(UnityAction _callback = null)
    {
        // �ݹ� �Լ��� ������� ������, UnityAction�� �޴� ������
        // �ʿ�� �ݹ��� ȣ���� �� �ֵ��� ���� ����
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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Play()
    {
        if(gameObject.activeSelf == false)
            gameObject.SetActive(true);
        

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
                if(particle.isPlaying == false)
                    particle.Play();
                playObjs++;
                //StartCoroutine(AfterAction(delay: 2f, action: () =>
                //{
                //    PlayEndCount++;
                //    //Debug.Log(obj.gameObject.name + " ��ƼŬ ��� �Ϸ�" +  PlayEndCount);
                //    //particle.gameObject.SetActive(false);
                //}));
            }
        }

        if(audioSet != null)
        {
            AudioSource audio = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            //audioSet?.Set(audio);
            audio.volume = audioSet.volume;
            audio.loop = audioSet.isLoop;
            if(audio.isPlaying == false)
            {
                if (audio.loop)
                    audio.Play();
                else
                    audio.PlayOneShot(audioSet.clip);
            }
            

            foreach (Transform child in transform)
            {
                AudioSource childAudio = child.GetComponent<AudioSource>();
                if (childAudio != null)
                {
                    //audioSet.Set(childAudio);
                    childAudio.volume = audioSet.volume;
                    childAudio.loop = audioSet.isLoop;
                    if (childAudio.isPlaying == false)
                    {
                        if(childAudio.loop)
                            childAudio.Play();
                        else
                            childAudio.PlayOneShot(audioSet.clip);
                    }
                    
                }
            }
        }

        StartCoroutine(DisableEvent(playObjs));
    }

    IEnumerator DisableEvent(int count)
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while (count > PlayEndCount)
        {
            int chiledCount = 0;
            foreach (Transform trf in transform)
            {
                if (trf.gameObject.activeSelf == false)
                    chiledCount++;
                else if (trf.GetComponents<Component>().Length == 1)
                    chiledCount++;
            }

            if(chiledCount == transform.childCount)
            {
                PlayEndCount = count; // ��� ��ƼŬ�� �����ٰ� ����
                break;
            }
            yield return wait;
        }

        if(PlayEndCount != count)
        {
            foreach (GameObject obj in objs)
            {
                if (obj.activeSelf)
                    obj.SetActive(false);
            }
        }

        callback?.Invoke(); // ��� ����Ʈ�� ���� �� �ݹ� ȣ��
        gameObject.SetActive(false);
    }

    IEnumerator AfterAction(UnityAction action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
