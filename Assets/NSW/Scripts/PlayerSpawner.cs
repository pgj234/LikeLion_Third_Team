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

    [Header("Map�� ���� ����Ʈ")]
    public MapSpawn[] mapSpawns;

    // ���� ���� �ִ� �� ID
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
    /// MapZoneTrigger���� ȣ���մϴ�.
    /// </summary>
    public void SetCurrentMap(string mapID)
    {
        _currentMapID = mapID;
    }

    /// <summary>
    /// �÷��̾ �׾��� �� �� �Լ��� ȣ���ϼ���.
    /// </summary>
    public void RespawnPlayer(GameObject player)
    {
        if (string.IsNullOrEmpty(_currentMapID))
        {
            Debug.LogWarning("CurrentMapID�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // mapID�� �´� spawnPoint�� ã�´�
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

        Debug.LogError($"Spawn Point�� ã�� �� �����ϴ�: {_currentMapID}");
    }
}
