using UnityEngine;


public class Enemy : MonoBehaviour, IDamageable
{
    [Tooltip("적의 체력")]
    public float health = 100f;


    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{name} took {amount} damage, remaining {health}");

        if (health <= 0f)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{name} died!");
        // 사망 효과, 점수 증가 등 추가 로직 가능
        Destroy(gameObject);
    }
}
