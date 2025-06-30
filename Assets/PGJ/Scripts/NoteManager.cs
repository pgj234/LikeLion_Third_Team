using UnityEngine;

public class NoteManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("OffsetTimingNote"))
        {
            if (false == GameManager.Instance.musicStart)
            {
                SoundManager.Instance.StopBGM();

                if (1 == GameManager.Instance.currentStage)
                {
                    SoundManager.Instance.PlayBGM(BGM.Test_Bgm);
                }
                else
                {
                    SoundManager.Instance.PlayBGM(BGM.LastStage_BGM);
                }

                GameManager.Instance.musicStart = true;

                col.gameObject.SetActive(false);
            }
        }

        if (col.CompareTag("Note"))
        {
            GameManager.Instance.SetRhythmTimimg(1);
        }
        else if (col.CompareTag("HalfNote"))
        {
            GameManager.Instance.SetRhythmTimimg(2);
        }
    }
}
