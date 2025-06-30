using UnityEngine;

public class Note : MonoBehaviour
{
    float noteSpd;

    [SerializeField] bool isLeftNote = true;

    BoxCollider2D col;

    void Awake()
    {
        if (isLeftNote)
        {
            col = GetComponent<BoxCollider2D>();
        }
    }

    void OnEnable()
    {
        noteSpd = 300 / (60f / GameManager.Instance.GetBPM());

        if (isLeftNote)
        {
            col.size = new Vector2((GameManager.Instance.GetBPM() / 1.8f) + 5, 1);
        }
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
