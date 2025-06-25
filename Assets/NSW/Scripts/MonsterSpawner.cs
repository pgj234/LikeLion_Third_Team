using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Detection")]
    [Tooltip("플레이어를 감지할 반경")]
    public float detectRadius = 10f;
    [Tooltip("플레이어 레이어만 감지하도록 설정하세요")]
    public LayerMask playerLayer;

    [Header("Spawn Settings")]
    [Tooltip("스폰할 몬스터 프리팹")]
    public GameObject monsterPrefab;
    [Tooltip("최대 생성 가능한 몬스터 수")]
    public int maxMonsters = 5;
    [Tooltip("몬스터 스폰 간격 (초)")]
    public float spawnInterval = 3f;
    [Tooltip("몬스터를 실제로 배치할 위치들")]
    public Transform[] spawnPoints;

    bool playerDetected = false;
    float timer = 0f;

    void Update()
    {
        // 1) 아직 플레이어를 감지하지 않았다면 OverlapSphere로 감지
        if (!playerDetected)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, playerLayer);
            if (hits.Length > 0)
            {
                playerDetected = true;
                timer = 0f;  // 바로 스폰이 시작되도록 초기화
            }
        }
        // 2) 플레이어 감지 이후엔 spawnInterval마다 스폰 시도
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
        // 이미 씬에 active 몬스터가 maxMonsters 이상이면 넘어감
        if (GameObject.FindObjectsOfType<Monster>().Length >= maxMonsters)
            return;

        // 랜덤하게 스폰 포인트 선택
        if (spawnPoints.Length == 0) return;
        Transform pt = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(monsterPrefab, pt.position, pt.rotation);
    }

    // Scene 뷰에서 감지 반경을 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
