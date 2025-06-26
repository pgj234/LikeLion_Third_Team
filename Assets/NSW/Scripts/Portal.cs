using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Header("�ڷ���Ʈ ���� ����")]
    public Transform destination;

    [Header("�� ���̾�")]
    public LayerMask groundLayer;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 1) Player ������Ʈ���� CharacterController�� ������
        // ���� �浹�� �ݶ��̴��� �پ��ִ� ������Ʈ ��ü���� CharacterController�� ã��
        CharacterController cc = other.GetComponent<CharacterController>();
        Transform playerTransform = other.transform; // �����̵� ��ų Transform

        // ���� �浹�� �ݶ��̴� ������Ʈ�� CharacterController�� ���ٸ�,
        // �� �θ� ������Ʈ���� ã�ƿ� (�÷��̾� ���� ������ ���� �ٸ�)
        if (cc == null)
        {
            // other.transform.parent�� null�� �ƴ� ��쿡�� �õ�
            if (other.transform.parent != null)
            {
                cc = other.transform.parent.GetComponent<CharacterController>();
                playerTransform = other.transform.parent; // CharacterController�� �ִ� ������Ʈ�� �����̵� ������� ����
            }
        }

        // �׷��� CharacterController�� ã�� ���ߴٸ�, ����ó�� Root���� �ٽ� �õ��ϰų� ���
        // �� �κ��� �÷��̾��� ���� ������ ���� ����ȭ
        // ���⼭�� ������ playerRoot ������ ����Ͽ�, CharacterController�� ã�� Transform�� �����
        if (cc == null)
        {
            // ������ �������� Root
            cc = other.transform.root.GetComponent<CharacterController>();
            playerTransform = other.transform.root;
        }

        if (cc == null)
        {
            Debug.LogWarning("Player ������Ʈ �Ǵ� �� �θ�/��Ʈ ������Ʈ���� CharacterController�� ã�� �� �����ϴ�. �����̵��� �� �����ϴ�.");
            return;
        }

        // 2) ���� ����ĳ��Ʈ�� ��Ȯ�� Y ��ǥ ã��
        Vector3 spawnPosition = destination.position;
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 5f, Vector3.down,
                            out hit, 10f, groundLayer))
        {
            spawnPosition.y = hit.point.y;
        }
        else
        {
            Debug.LogWarning("��Ż ���� ���� �Ʒ��� ����(Ground Layer)�� �����ϴ�! destination�� Y ��ǥ�� ����մϴ�.");
        }

        // 3) �����̵�
        cc.enabled = false; // CharacterController ��Ȱ��ȭ
        playerTransform.position = spawnPosition; // CharacterController�� �ִ� ������Ʈ�� ��ġ�� �����̵�
        cc.enabled = true;  // CharacterController �ٽ� Ȱ��ȭ
    }
}