using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] protected float speed;                  //이동속도
    [SerializeField] protected float lifeTime = 5f;          //생존시간
    [SerializeField] protected float timer = 0f;             //타이머
    [SerializeField] protected int damage = 1;               //데미지
    protected Vector3 dir;                                   //방향
    protected WeaponBase weapon;                             //무기

    [Header("Effect")]
    [SerializeField] private GameObject hitEffect;           //맞았을 때 이펙트
    [SerializeField] private AudioSource hitSound;           //맞았을 때 사운드
    [SerializeField] private HitEffectObj hitObj;            //맞았을 때 커스텀 이펙트 오브젝트

    /// <summary> 생성시 호출 </summary>
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

    /// <summary> 지속적인 움직임 </summary>
    protected virtual void Update()
    {
        // 생존시간이 설정되어 있다면 타이머를 증가시키고, 시간이 초과되면 비활성화
        if (lifeTime > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime)
                gameObject.SetActive(false);
        }

        // 이동
        transform.position += dir.normalized * speed * Time.deltaTime;

    }

    /// <summary>  벽에 부딪치면 삭제 </summary>
    protected virtual void OnTriggerEnter(Collider col)
    {
        weapon.hitAction?.Invoke(col); // 무기에서 설정된 맞았을 때 액션 호출
        HitEffectPlay();
    }

    /// <summary> 오브젝트 풀 비활성화 호출 </summary>
    protected virtual void OnDisable()
    {

    }

    protected virtual void HitEffectPlay()
    {
        // 파티클 오브젝트 일경우?
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            if(effect.GetComponent<ParticleSystem>() != null && effect.GetComponent<ParticleSystem>().isPlaying == false)
            {
                effect.GetComponent<ParticleSystem>().Play();
            }
        }
        // 오디오 소스가 있다면 재생
        if (hitSound != null)
        {
            hitSound.Play();
        }
        // 커스텀 이펙트 오브젝트가 있다면 설정 후 재생
        if (hitObj != null)
        {
            hitObj.ObjSet(_callback:() => weapon?.effectEndAction?.Invoke());
            hitObj.Play();
        }
    }
}
