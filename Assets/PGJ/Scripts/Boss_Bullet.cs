using System.Collections;
using UnityEngine;

public class Boss_Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] int dmg;

    Renderer matRenderer;
    Color baseColor;

    bool realShot = false;

    Player target;

    float lightTimer = 0.7f;

    void Awake()
    {
        matRenderer = GetComponent<Renderer>();

        baseColor = matRenderer.material.color;
    }

    void Start()
    {
        Destroy(gameObject, 4);
    }

    internal void RealShot(Player _target)
    {
        target = _target;
        realShot = true;

        if (false == target.GetIsDash())
        {
            target.GetDamage(dmg);
        }
    }

    private void Update()
    {
        if (lightTimer > 0)
        {
            lightTimer -= Time.deltaTime;

            //Color finalColor = baseColor * Mathf.LinearToGammaSpace(lightTimer * 1000);
            //matRenderer.material.SetColor("_EmissionColor", finalColor);
        }

        if (false == realShot)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, bulletSpeed * Time.deltaTime);
    }
}
