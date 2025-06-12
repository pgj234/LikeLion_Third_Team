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
                SoundManager.Instance.PlayBGM(BGM.Test_Bgm);
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

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Note") || col.CompareTag("HalfNote"))
        {
            GameManager.Instance.NotePush();

            Debug.Log("놓침...");
        }
    }
}
