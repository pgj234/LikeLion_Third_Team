using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] protected float speed;                  //�̵��ӵ�
    [SerializeField] protected float lifeTime = 5f;          //�����ð�
    [SerializeField] protected float timer = 0f;    //Ÿ�̸�
    [SerializeField] protected int damage = 1;               //������
    protected Vector3 dir;                                   //����
    protected WeaponBase weapon;                             //����
    protected Entity shooter;                                //�߻��� (Entity)

    [Header("Component")]
    [SerializeField] protected Rigidbody rb;                 //������ٵ� ������Ʈ
    [SerializeField] protected Collider col;                 //�ݶ��̴� ������Ʈ

    [Header("Effect")]
    [SerializeField] protected GameObject hitEffect;         //�¾��� �� ����Ʈ
    [SerializeField] protected AudioSource hitSound;         //�¾��� �� ����
    [SerializeField] protected AudioClip hitAudioClip;       //�¾��� �� ���� Ŭ��
    [SerializeField] protected HitEffectObj hitObj;          //�¾��� �� Ŀ���� ����Ʈ ������Ʈ

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    /// <summary> ������ ȣ�� </summary>
    public virtual Bullet Set(Vector3 _position, Quaternion _lotation, Vector3 _direction, float _speed,
        float _lifeTime = default, int _damage = 1, WeaponBase _weapon = null, GameObject _hitEffect = null, AudioSource _hitAudio = null, HitEffectObj _hitObj = null, AudioClip _hitAudioClip = null, Entity _shooter = null)
    {
        transform.position = _position;
        transform.rotation = _lotation;
        GetComponent<Rigidbody>().linearVelocity = _direction.normalized * _speed;

        lifeTime = _lifeTime;
        dir = _direction;
        speed = _speed;
        damage = _damage;
        weapon = _weapon;
        shooter = _shooter;
        timer = 0f;

        hitEffect = _hitEffect;
        hitSound = _hitAudio;
        hitObj = _hitObj;
        hitAudioClip = _hitAudioClip;

        return this;
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
        //transform.position += dir.normalized * speed * Time.deltaTime;
        rb.linearVelocity = dir.normalized * speed;
        transform.rotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up); // �̵� �������� ȸ��

    }

    /// <summary>  ���� �ε�ġ�� ���� </summary>
    protected virtual void OnTriggerEnter(Collider col)
    {
        if(col.GetComponent<Entity>() != null)
        {
            Entity entity = col.GetComponent<Entity>();
            //entity.Damage(damage); // ������ ����
        }

        //weapon.hitAction?.Invoke(col); // ���⿡�� ������ �¾��� �� �׼� ȣ��
        HitEffectPlay();
        gameObject.SetActive(false);
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
            if (effect.activeSelf == false)
                effect.SetActive(true);
            if (effect.GetComponent<ParticleSystem>() != null && effect.GetComponent<ParticleSystem>().isPlaying == false)
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
            //hitObj.ObjSet(_callback:() => weapon?.effectEndAction?.Invoke());
            GameObject obj = Instantiate(hitObj.gameObject, transform.position, shooter != null ? Quaternion.LookRotation(shooter.transform.position) : Quaternion.identity);
            //obj.transform.forward = -transform.forward;
            if(obj.activeSelf == false)
                obj.SetActive(true);
            obj.GetComponent<HitEffectObj>()?.Play();
        }
    }
}
