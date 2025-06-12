using UnityEngine;

public class CenterNoteManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Note") || col.CompareTag("HalfNote"))
        {
            GameManager.Instance.NotePush();

            Debug.Log("놓침...");
        }
    }
}
