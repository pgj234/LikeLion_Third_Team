using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct MapSpawn
    {
        public string mapID;
        public Transform spawnPoint;
    }

    public static PlayerSpawner Instance { get; private set; }

    [Header("Map별 스폰 포인트")]
    public MapSpawn[] mapSpawns;

    // 현재 들어와 있는 맵 ID
    private string _currentMapID = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// MapZoneTrigger에서 호출합니다.
    /// </summary>
    public void SetCurrentMap(string mapID)
    {
        _currentMapID = mapID;
    }

    /// <summary>
    /// 플레이어가 죽었을 때 이 함수를 호출하세요.
    /// </summary>
    public void RespawnPlayer(GameObject player)
    {
        if (string.IsNullOrEmpty(_currentMapID))
        {
            Debug.LogWarning("CurrentMapID가 설정되지 않았습니다!");
            return;
        }

        // mapID에 맞는 spawnPoint를 찾는다
        foreach (var mp in mapSpawns)
        {
            if (mp.mapID == _currentMapID)
            {
                player.transform.position = mp.spawnPoint.position;
                player.transform.rotation = mp.spawnPoint.rotation;
                Debug.Log($"Respawned at {mp.mapID} spawn");
                return;
            }
        }

        Debug.LogError($"Spawn Point를 찾을 수 없습니다: {_currentMapID}");
    }
}
