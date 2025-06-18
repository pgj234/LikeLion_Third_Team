using UnityEngine;


public class Enemy : MonoBehaviour, IDamageable
{
    [Tooltip("���� ü��")]
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
        // ��� ȿ��, ���� ���� �� �߰� ���� ����
        Destroy(gameObject);
    }
}
