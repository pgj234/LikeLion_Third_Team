using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Header("텔레포트 도착 지점")]
    public Transform destination;

    [Header("땅 레이어")]
    public LayerMask groundLayer;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) Player 오브젝트에서 CharacterController를 가져옴
        // 먼저 충돌한 콜라이더가 붙어있는 오브젝트 자체에서 CharacterController를 찾기
        CharacterController cc = other.GetComponent<CharacterController>();
        Transform playerTransform = other.transform; // 순간이동 시킬 Transform

        // 만약 충돌한 콜라이더 오브젝트에 CharacterController가 없다면,
        // 그 부모 오브젝트에서 찾아옴 (플레이어 계층 구조에 따라 다름)
        if (cc == null)
        {
            // other.transform.parent가 null이 아닐 경우에만 시도
            if (other.transform.parent != null)
            {
                cc = other.transform.parent.GetComponent<CharacterController>();
                playerTransform = other.transform.parent; // CharacterController가 있는 오브젝트를 순간이동 대상으로 설정
            }
        }

        // 그래도 CharacterController를 찾지 못했다면, 원래처럼 Root에서 다시 시도하거나 경고
        // 이 부분은 플레이어의 계층 구조에 따라 최적화
        // 여기서는 기존의 playerRoot 로직을 대신하여, CharacterController를 찾은 Transform을 사용함
        if (cc == null)
        {
            // 최후의 수단으로 Root
            cc = other.transform.root.GetComponent<CharacterController>();
            playerTransform = other.transform.root;
        }

        if (cc == null)
        {
            Debug.LogWarning("Player 오브젝트 또는 그 부모/루트 오브젝트에서 CharacterController를 찾을 수 없습니다. 순간이동할 수 없습니다.");
            return;
        }

        // 2) 지면 레이캐스트로 정확한 Y 좌표 찾기
        Vector3 spawnPosition = destination.position;
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 5f, Vector3.down,
                            out hit, 10f, groundLayer))
        {
            spawnPosition.y = hit.point.y;
        }
        else
        {
            Debug.LogWarning("포탈 도착 지점 아래에 지면(Ground Layer)이 없습니다! destination의 Y 좌표를 사용합니다.");
        }

        // 3) 순간이동
        cc.enabled = false; // CharacterController 비활성화
        playerTransform.position = spawnPosition; // CharacterController가 있는 오브젝트의 위치를 순간이동
        cc.enabled = true;  // CharacterController 다시 활성화
    }
}