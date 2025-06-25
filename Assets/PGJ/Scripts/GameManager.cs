using System.Collections.Generic;
using UnityEngine;

enum PlayType
{
    Loading,
    Playing,
    Pause,
    End
}

public class GameManager : SingletonBehaviour<GameManager>
{
    int score;

    const int maxCombo = 16;
    int combo;

    internal int bpm { get; private set; }
    double currentTime = 0d;

    [SerializeField] Transform leftLineTr = null;
    [SerializeField] Transform rightLineTr = null;

    [SerializeField] Transform leftNoteAppearTr = null;
    [SerializeField] Transform rightNoteAppearTr = null;

    [SerializeField] GameObject leftNoteObj = null;
    [SerializeField] GameObject rightNoteObj = null;

    [SerializeField] GameObject leftHalfNoteObj = null;
    [SerializeField] GameObject rightHalfNoteObj = null;

    internal bool musicStart = false;

    int rhythmTimingNum = 0;        // 0 : 박자 타이밍 X, 1 : 정박 타이밍, 2 : 반박 타이밍
    bool noteDisable = false;       // true : 노트 꺼질 수 있는 상태 (막 누르면 저 멀리 있는 노트도 다 없어지는 현상 방지)

    // 노트 오브젝트 저장소
    Queue<GameObject> leftNoteObjQueue = new Queue<GameObject>();
    Queue<GameObject> rightNoteObjQueue = new Queue<GameObject>();

    // 노트 오브젝트 사용중인 큐
    Queue<GameObject> leftNoteQueue = new Queue<GameObject>();
    Queue<GameObject> rightNoteQueue = new Queue<GameObject>();

    EventManager eventManager;

    protected override void Init()
    {
        base.Init();

        eventManager = EventManager.Instance;

        bpm = 128;      // 테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트테스트
        for (int i=0; i<20; i++)
        {
            if (1 == i % 2)     // 반박 생성
            {
                GameObject noteObj = Instantiate(leftHalfNoteObj, leftNoteAppearTr.position, Quaternion.identity, leftLineTr);
                noteObj.SetActive(false);
                leftNoteObjQueue.Enqueue(noteObj);

                noteObj = Instantiate(rightHalfNoteObj, rightNoteAppearTr.position, Quaternion.identity, rightLineTr);
                noteObj.SetActive(false);
                rightNoteObjQueue.Enqueue(noteObj);
            }
            else                // 정박 생성
            {
                GameObject noteObj = Instantiate(leftNoteObj, leftNoteAppearTr.position, Quaternion.identity, leftLineTr);
                noteObj.SetActive(false);
                leftNoteObjQueue.Enqueue(noteObj);

                noteObj = Instantiate(rightNoteObj, rightNoteAppearTr.position, Quaternion.identity, rightLineTr);

                if (0 == i)     // 가장 처음 정박은 오프셋 노트 활성화 (RightArrow 껄로)
                {
                    noteObj.transform.Find("OffsetTimingNote").gameObject.SetActive(true);
                }

                noteObj.SetActive(false);
                rightNoteObjQueue.Enqueue(noteObj);
            }
        }
    }

    //void Start()
    //{
    //    setting   // fps 고정
    //}

    internal void SetBPM(int _bpm)
    {
        bpm = _bpm;
    }

    internal int GetBPM()
    {
        return bpm;
    }

    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 30d / bpm)
        {
            NotePull();

            currentTime -= 30d / bpm;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // 막 누르면 저 멀리 있는 노트도 다 없어지는 현상 방지하는 변수 설정 (리듬 타이밍에 가까운 노트만 상호작용 가능하게)
    internal void SetNoteDisable(bool _isOK)
    {
        noteDisable = _isOK;
    }

    internal bool GetNoteDisable()
    {
        return noteDisable;
    }

    internal void SetRhythmTimimg(int _timingNum)
    {
        rhythmTimingNum = _timingNum;
    }

    public int RhythmCheck()
    {
        return rhythmTimingNum;
    }

    internal void PauseOnAndOff()
    {
    
    }

    // 콤보 추가
    internal void AddCombo()
    {
        if (combo < maxCombo)
        {
            combo++;
        }
        Debug.Log("콤보 : " + combo);

        eventManager.PlayerComboRefreshEvent(combo);
    }

    // 콤보 반토막
    internal void SetHalfCombo()
    {
        if (combo > 0)
        {
            combo /= 2;
        }
        Debug.Log("콤보 : " + combo);

        eventManager.PlayerComboRefreshEvent(combo);
    }

    #region 노트 풀링
    internal void NotePull()
    {
        GameObject noteObj = leftNoteObjQueue.Dequeue();
        leftNoteQueue.Enqueue(noteObj);
        noteObj.transform.position = leftNoteAppearTr.position;
        noteObj.SetActive(true);

        noteObj = rightNoteObjQueue.Dequeue();
        rightNoteQueue.Enqueue(noteObj);
        noteObj.transform.position = rightNoteAppearTr.position;
        noteObj.SetActive(true);
    }

    internal void NotePush()
    {
        if (false == noteDisable)
        {
            return;
        }

        SetNoteDisable(false);

        GameObject noteObj = leftNoteQueue.Dequeue();
        leftNoteObjQueue.Enqueue(noteObj);
        noteObj.SetActive(false);
        noteObj.transform.position = leftNoteAppearTr.position;

        noteObj = rightNoteQueue.Dequeue();
        rightNoteObjQueue.Enqueue(noteObj);
        noteObj.SetActive(false);
        noteObj.transform.position = rightNoteAppearTr.position;

        SetRhythmTimimg(0);
    }
    #endregion
}
