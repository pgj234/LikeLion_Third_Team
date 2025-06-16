using UnityEngine;

public class PlayerControllerTest_PGJ : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (false == GameManager.Instance.musicStart || false == GameManager.Instance.GetNoteDisable())
            {
                return;
            }

            if (1 == GameManager.Instance.RhythmCheck())
            {
                Debug.Log("정박 성공!");
                EventManager.Instance.PlayerAddComboEvent();
            }
            else if (2 == GameManager.Instance.RhythmCheck())
            {
                Debug.Log("반박 성공!");
                EventManager.Instance.PlayerAddComboEvent();
            }
            else
            {
                Debug.Log("박자 타이밍 실패...");
                SoundManager.Instance.PlaySFX(SFX.RhythmFail);
                EventManager.Instance.PlayerReduceComboEvent();
            }

            GameManager.Instance.NotePush();
        }
    }
}
