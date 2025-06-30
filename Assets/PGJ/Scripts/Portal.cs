using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] StageManager stageManager;
    [SerializeField] bool toStage3;

    [Header("2 스테이지 출발 지점")]
    [SerializeField] Transform stage2_Start_Tr;

    [Space(5)]
    [Header("3 스테이지 출발 지점")]
    [SerializeField] Transform stage3_Start_Tr;

    void OnTriggerEnter(Collider col)
    {
        if (false == toStage3)
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<Player>().PortalUse();
                col.transform.position = stage2_Start_Tr.position;
                stageManager.levelStart = true;
                stageManager.FirstSpawn(2);

                Destroy(gameObject);
            }
        }
        else
        {
            if (col.CompareTag("Player"))
            {
                col.GetComponent<Player>().PortalUse();
                col.transform.position = stage3_Start_Tr.position;
                stageManager.levelStart = true;
                stageManager.FirstSpawn(3);

                GameManager.Instance.musicStart = false;

                GameManager.Instance.SetBPM(128);
                GameManager.Instance.BGM_Change();

                Destroy(gameObject);
            }
        }
    }
}
