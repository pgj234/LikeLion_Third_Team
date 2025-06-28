using UnityEngine;

public class BossHP : MonoBehaviour
{
    Transform camera;

    void Awake()
    {
        camera = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(camera);
    }
}
