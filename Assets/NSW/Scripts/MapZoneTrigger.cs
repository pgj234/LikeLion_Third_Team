using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MapZoneTrigger : MonoBehaviour
{
    [Tooltip("�� Ʈ���Ű� ���� ���� ID (PlayerSpawner.MapSpawns�� �����ּ���)")]
    public string mapID;

    private void Reset()
    {
        // Collider�� Ʈ���ŷ� �ڵ� ����
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerSpawner.Instance.SetCurrentMap(mapID);
            Debug.Log($"Entered Zone: {mapID}");
        }
    }
}
