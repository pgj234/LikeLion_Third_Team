using UnityEngine;

public class Note : MonoBehaviour
{
    float noteSpd;

    [SerializeField] bool isLeftNote = true;

    void OnEnable()
    {
        noteSpd = 300 / (60f / GameManager.Instance.GetBPM());
    }

    void Update()
    {
        if (true == isLeftNote)
        {
            transform.Translate(Vector3.right * noteSpd * Time.deltaTime);

            if (false == GameManager.Instance.GetNoteDisable())
            {
                if (-200 < transform.localPosition.x)
                {
                    GameManager.Instance.SetNoteDisable(true);
                }
            }
        }
        else
        {
            transform.Translate(-Vector3.right * noteSpd * Time.deltaTime);
        }
    }
}
