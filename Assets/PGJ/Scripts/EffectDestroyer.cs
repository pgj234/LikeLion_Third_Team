using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    [SerializeField] float destroyDelayTime;

    void Start()
    {
        Destroy(gameObject, destroyDelayTime);
    }
}
