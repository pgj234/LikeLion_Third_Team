using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    int bpm = 0;
    double currentTime = 0d;

    [SerializeField] Transform noteAppear = null;
    [SerializeField] GameObject NoteObj = null;

    protected override void Init()
    {
        base.Init();


    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bpm)
        {
            GameObject noteObj = Instantiate(NoteObj, noteAppear.position, Quaternion.identity);
            currentTime -= 60d / bpm;
        }
    }
}
