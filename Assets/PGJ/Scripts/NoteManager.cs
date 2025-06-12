using UnityEngine;

public class NoteManager : MonoBehaviour
{
    bool musicStart = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Note"))
        {
            if (false == musicStart)
            {
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM(BGM.Test_Bgm);
                musicStart = true;
            }

            GameManager.Instance.SetRhythmTimimg(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Note"))
        {
            GameManager.Instance.NotePush();
        }
    }
}
