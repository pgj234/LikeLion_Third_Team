using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MapZoneTrigger : MonoBehaviour
{
    [Tooltip("이 트리거가 속한 맵의 ID (PlayerSpawner.MapSpawns에 맞춰주세요)")]
    public string mapID;

    private void Reset()
    {
        // Collider를 트리거로 자동 설정
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
