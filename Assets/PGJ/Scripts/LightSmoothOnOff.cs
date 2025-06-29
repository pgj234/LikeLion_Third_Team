using UnityEngine;

public class LightSmoothOnOff : MonoBehaviour
{
    public Light pointLight;
    public float duration = 1.0f;   // 전환 시간
    public float maxIntensity = 1.0f;

    void Update()
    {
        maxIntensity -= Time.deltaTime * 50;
        pointLight.intensity = Mathf.Lerp(maxIntensity, 0, duration);
    }
}
