using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    int bpm = 0;
    double currentTime = 0d;

    [SerializeField] Transform noteAppear = null;
    [SerializeField] GameObject noteObj = null;

    protected override void Init()
    {
        base.Init();


    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bpm)
        {
            GameObject obj = Instantiate(noteObj, noteAppear.position, Quaternion.identity);
            currentTime -= 60d / bpm;
        }
    }

    public bool RhythmCheck()
    {
        return true;
    }
}
