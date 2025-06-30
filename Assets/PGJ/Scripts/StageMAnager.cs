using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("1 스테이지 몬스터")]
    [SerializeField] Entity[] stage1_Wave_01;
    [SerializeField] Entity[] stage1_Wave_02;

    [Space(7)]
    [Header("2 스테이지 몬스터")]
    [SerializeField] Entity[] stage2_Wave_01;
    [SerializeField] Entity[] stage2_Wave_02;
    [SerializeField] Entity[] stage2_Wave_03;

    [Space(7)]
    [Header("3 스테이지 몬스터")]
    [SerializeField] Entity[] stage3_Wave_01;
    [SerializeField] Entity[] stage3_Wave_02;
    [SerializeField] Entity[] stage3_Wave_03;
    [SerializeField] Entity[] stage3_Wave_04;
    [SerializeField] Entity[] stage3_Wave_05;

    [Space(10)]
    [Header("1 스테이지 출구")]
    [SerializeField] GameObject stage1_Exit_Obj;

    [Header("2 스테이지 출구")]
    [SerializeField] GameObject stage2_Exit_Obj;

    [Space(10)]
    [Header("1스테이지 보상")]
    [SerializeField] GameObject stage1_Reward;
    [Header("2스테이지 보상")]
    [SerializeField] GameObject stage2_Reward;
    [Header("3스테이지 중간 보상")]
    [SerializeField] GameObject stage3_Reward;

    int currentWaveIdx;

    float checkDelayTime = 3;
    float checkTimer;

    internal bool levelStart = false;
    bool bossClear = false;

    void Start()
    {
        Init();
    }

    void Init()
    {
        GameManager.Instance.currentStage = 1;
        currentWaveIdx = 0;

        checkTimer = 0;

        levelStart = true;

        FirstSpawn(1);
    }

    void Update()
    {
        checkTimer += Time.deltaTime;

        if (checkTimer > checkDelayTime)
        {
            checkTimer = 0;

            if (true == levelStart)
            {
                MonsterDieCheck();
            }
        }
    }

    void MonsterDieCheck()
    {
        bool nextLevelGo = true;

        switch (GameManager.Instance.currentStage)
        {
            case 1:
                switch (currentWaveIdx)
                {
                    case 0:
                        for (int i = 0; i < stage1_Wave_01.Length; i++)
                        {
                            Debug.Log(stage1_Wave_01[i].name + " : " + stage1_Wave_01[i].GetIsDie());
                            if (false == stage1_Wave_01[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 1:
                        for (int i = 0; i < stage1_Wave_02.Length; i++)
                        {
                            if (false == stage1_Wave_02[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;
                }

                break;

            case 2:
                switch (currentWaveIdx)
                {
                    case 0:
                        for (int i = 0; i < stage2_Wave_01.Length; i++)
                        {
                            if (false == stage2_Wave_01[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 1:
                        for (int i = 0; i < stage2_Wave_02.Length; i++)
                        {
                            if (false == stage2_Wave_02[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 2:
                        for (int i = 0; i < stage2_Wave_03.Length; i++)
                        {
                            if (false == stage2_Wave_03[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;
                }

                break;

            case 3:
                switch (currentWaveIdx)
                {
                    case 0:
                        for (int i = 0; i < stage3_Wave_01.Length; i++)
                        {
                            if (false == stage3_Wave_01[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 1:
                        for (int i = 0; i < stage3_Wave_02.Length; i++)
                        {
                            if (false == stage3_Wave_02[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 2:
                        for (int i = 0; i < stage3_Wave_03.Length; i++)
                        {
                            if (false == stage3_Wave_03[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 3:
                        for (int i = 0; i < stage3_Wave_04.Length; i++)
                        {
                            if (false == stage3_Wave_04[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                break;
                            }
                        }

                        break;

                    case 4:
                        for (int i = 0; i < stage3_Wave_04.Length; i++)
                        {
                            if (false == stage3_Wave_04[i].GetIsDie())
                            {
                                nextLevelGo = false;
                                bossClear = true;
                                break;
                            }
                        }

                        break;
                }

                break;
        }

        if (true == nextLevelGo)
        {
            NextSpawn();
        }
    }

    internal void FirstSpawn(int stageNum)
    {
        switch (stageNum)
        {
            case 1:
                for (int i = 0; i < stage1_Wave_01.Length; i++)
                {
                    stage1_Wave_01[i].gameObject.SetActive(true);
                }
                break;

            case 2:
                for (int i = 0; i < stage2_Wave_01.Length; i++)
                {
                    stage2_Wave_01[i].gameObject.SetActive(true);
                }
                break;

            case 3:
                for (int i = 0; i < stage3_Wave_01.Length; i++)
                {
                    stage3_Wave_01[i].gameObject.SetActive(true);
                }
                break;
        }
    }

    void NextSpawn()
    {
        if (bossClear)
        {
            Debug.Log("클리어!");
            return;
        }

        // 1 스테이지 끝
        if (GameManager.Instance.currentStage == 1 && currentWaveIdx == 1)
        {
            GameManager.Instance.currentStage++;
            currentWaveIdx = 0;

            levelStart = false;

            stage1_Reward.SetActive(true);

            stage1_Exit_Obj.SetActive(true);
        }
        else if (GameManager.Instance.currentStage == 2 && currentWaveIdx == 2)
        {
            GameManager.Instance.currentStage++;
            currentWaveIdx = 0;

            levelStart = false;

            stage2_Reward.SetActive(true);

            stage2_Exit_Obj.SetActive(true);
        }
        else
        {
            currentWaveIdx++;
        }

        switch (GameManager.Instance.currentStage)
        {
            case 1:
                switch (currentWaveIdx)
                {
                    case 1:
                        for (int i = 0; i < stage1_Wave_02.Length; i++)
                        {
                            stage1_Wave_02[i].gameObject.SetActive(true);
                        }

                        break;
                }

                break;

            case 2:
                switch (currentWaveIdx)
                {
                    case 1:
                        for (int i = 0; i < stage2_Wave_02.Length; i++)
                        {
                            stage2_Wave_02[i].gameObject.SetActive(true);
                        }

                        break;

                    case 2:
                        for (int i = 0; i < stage2_Wave_03.Length; i++)
                        {
                            stage2_Wave_03[i].gameObject.SetActive(true);
                        }

                        break;
                }

                break;

            case 3:
                switch (currentWaveIdx)
                {
                    case 1:
                        for (int i = 0; i < stage3_Wave_02.Length; i++)
                        {
                            stage3_Wave_02[i].gameObject.SetActive(true);
                        }

                        break;

                    case 2:
                        for (int i = 0; i < stage3_Wave_03.Length; i++)
                        {
                            stage3_Wave_03[i].gameObject.SetActive(true);
                        }

                        stage3_Reward.SetActive(true);

                        break;

                    case 3:
                        for (int i = 0; i < stage3_Wave_04.Length; i++)
                        {
                            stage3_Wave_04[i].gameObject.SetActive(true);
                        }

                        break;

                    case 4:
                        for (int i = 0; i < stage3_Wave_05.Length; i++)
                        {
                            stage3_Wave_05[i].gameObject.SetActive(true);
                        }

                        break;
                }

                break;
        }
    }
}
