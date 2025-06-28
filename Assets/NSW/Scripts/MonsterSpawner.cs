using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("�÷��̾ ������ �ݰ�")]
    public float detectRadius = 10f;
    [Tooltip("�÷��̾� ���̾ �����ϵ��� �����ϼ���")]
    public LayerMask playerLayer;

    [Header("Spawn Settings")]
    [Tooltip("������ ���� ������")]
    public GameObject monsterPrefab;
    [Tooltip("�ִ� ���� ������ ���� ��")]
    public int maxMonsters = 5;
    [Tooltip("���� ���� ���� (��)")]
    public float spawnInterval = 3f;
    [Tooltip("���͸� ������ ��ġ�� ��ġ��")]
    public Transform[] spawnPoints;

    bool playerDetected = false;
    float timer = 0f;

    void Update()
    {
        // 1) ���� �÷��̾ �������� �ʾҴٸ� OverlapSphere�� ����
        if (!playerDetected)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);
            if (hits.Length > 0)
            {
                playerDetected = true;
                timer = 0f;  // �ٷ� ������ ���۵ǵ��� �ʱ�ȭ
            }
        }
        // 2) �÷��̾� ���� ���Ŀ� spawnInterval���� ���� �õ�
        else
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                TrySpawn();
                timer = 0f;
            }
        }
    }

    void TrySpawn()
    {
        // �̹� ���� active ���Ͱ� maxMonsters �̻��̸� �Ѿ
        if (GameObject.FindObjectsOfType<Monster>().Length >= maxMonsters)
            return;

        // �����ϰ� ���� ����Ʈ ����
        if (spawnPoints.Length == 0) return;
        Transform pt = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(monsterPrefab, pt.position, pt.rotation);
    }

    // Scene �信�� ���� �ݰ��� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
