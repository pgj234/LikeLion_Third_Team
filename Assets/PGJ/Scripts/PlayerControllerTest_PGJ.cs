using UnityEngine;

public class PlayerControllerTest_PGJ : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (true == GameManager.Instance.RhythmCheck())
            {
                Debug.Log("리듬 성공!");

                GameManager.Instance.NotePush();
            }
            else
            {
                Debug.Log("앗...");

                GameManager.Instance.NotePush();
            }
        }
    }
}
