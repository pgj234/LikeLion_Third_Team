using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] protected float speed;                  //�̵��ӵ�
    [SerializeField] protected float lifeTime = 5f;          //�����ð�
    [SerializeField] protected float timer = 0f;             //Ÿ�̸�
    [SerializeField] protected int damage = 1;               //������
    protected Vector3 dir;                                   //����
    protected WeaponBase weapon;                             //����

    [Header("Effect")]
    [SerializeField] private GameObject hitEffect;           //�¾��� �� ����Ʈ
    [SerializeField] private AudioSource hitSound;           //�¾��� �� ����
    [SerializeField] private HitEffectObj hitObj;            //�¾��� �� Ŀ���� ����Ʈ ������Ʈ

    /// <summary> ������ ȣ�� </summary>
    public Bullet(Vector3 _position, Quaternion _lotation, Vector3 _direction, float _speed,
        float _lifeTime = default, int damage = 1, WeaponBase _weapon = null)
    {
        transform.position = _position;
        transform.rotation = _lotation;
        GetComponent<Rigidbody>().linearVelocity = _direction.normalized * _speed;

        lifeTime = _lifeTime;
        dir = _direction;
        speed = _speed;
        this.damage = damage;
        weapon = _weapon;
        timer = 0f;

    }

    /// <summary> �������� ������ </summary>
    protected virtual void Update()
    {
        // �����ð��� �����Ǿ� �ִٸ� Ÿ�̸Ӹ� ������Ű��, �ð��� �ʰ��Ǹ� ��Ȱ��ȭ
        if (lifeTime > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime)
                gameObject.SetActive(false);
        }

        // �̵�
        transform.position += dir.normalized * speed * Time.deltaTime;

    }

    /// <summary>  ���� �ε�ġ�� ���� </summary>
    protected virtual void OnTriggerEnter(Collider col)
    {
        weapon.hitAction?.Invoke(col); // ���⿡�� ������ �¾��� �� �׼� ȣ��
        HitEffectPlay();
    }

    /// <summary> ������Ʈ Ǯ ��Ȱ��ȭ ȣ�� </summary>
    protected virtual void OnDisable()
    {

    }

    protected virtual void HitEffectPlay()
    {
        // ��ƼŬ ������Ʈ �ϰ��?
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            if(effect.GetComponent<ParticleSystem>() != null && effect.GetComponent<ParticleSystem>().isPlaying == false)
            {
                effect.GetComponent<ParticleSystem>().Play();
            }
        }
        // ����� �ҽ��� �ִٸ� ���
        if (hitSound != null)
        {
            hitSound.Play();
        }
        // Ŀ���� ����Ʈ ������Ʈ�� �ִٸ� ���� �� ���
        if (hitObj != null)
        {
            hitObj.ObjSet(_callback:() => weapon?.effectEndAction?.Invoke());
            hitObj.Play();
        }
    }
}
