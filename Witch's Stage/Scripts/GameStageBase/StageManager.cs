using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public StageCharacterController CharacterController { get => characterController; }
    [SerializeField] private StageCharacterController characterController;
    public SoundController SoundControllerIns { get => soundController; }
    [SerializeField] private SoundController soundController;
    public float Speed { get => speed; private set => speed = value; }
    public AudioSource audioSource;
    private float speed;
    public JudgeInfoData judgeInfoData;
    [SerializeField] private TextMeshProUGUI scoreText;

    public float Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = ((int)score).ToString();
        }
    }

    private float score = 0f;

    public float ScoreUnit
    {
        get
        {
            return scoreUnit;
        }
        set
        {
            scoreUnit = value;
        }
    }

    private float scoreUnit = 100f;

    public float JudgeDistance
    {
        get
        {
            return judgeDistance;
        }
        set
        {
            judgeDistance = value;
        }
    }
    private float judgeDistance = 0f;

    public float NowHP
    {
        get
        {
            return nowHP;
        }
        set
        {
            if (value >= maxHP)
                nowHP = maxHP;
            else if (value <= 0)
                nowHP = 0;
            else
                nowHP = value;

            JudgeTrailerController.Instance.outlineRenderer.material.SetFloat("_FillValue", nowHP / maxHP);

            if (nowHP / maxHP <= 0.5f)
            {
                CharacterController.warningFlg = true;
            }
            else
            {
                CharacterController.warningFlg = false;
            }

            if (nowHP <= 0)
            {
                gameStatus = GameState.End;
                FindObjectOfType<CharacterSpriteController>().SetCharacterStatus(CharacterSpriteStatus.Idle);
                //TODO 게임 오버 처리
                FindObjectOfType<GameStatusNoticeManager>().GameOver();
            }
        }
    }

    private TransferStageData stageData;
    public TransferStageData StageData
    {
        get { return stageData; }
        set { stageData = value; }
    }

    private float nowHP = 0f;

    [SerializeField] private float maxHP = 3f;
    [SerializeField] private GameObject warningObj;

    [HideInInspector] public List<NoteController> noteList = new();
    [HideInInspector] public int noteIndex = 0;
    [HideInInspector] public List<Vector2> railNodes;

    public bool GameStarted
    {
        get
        {
            if (gameStatus == GameState.Playing) return true; else return false;
        }
    }
    [HideInInspector] public float playTime = 0f;
    [HideInInspector] public GameState gameStatus = GameState.Ready;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Speed = StageData.speedMultiplier * StageConstant.DEFAULT_SPEED;
        StageData = FindObjectOfType<SceneLoadManager>().StageData;

        audioSource.clip = StageData.music;
    }

    void Start()
    {
        // 속도에 따라서 세이프 에어리어의 크기를 조절한다.
        Cursor.lockState = CursorLockMode.Confined;

        // 게임 결과 창을 초기화한다.
        GameResultManager.Instance.InitResultPanel();
        //TODO 음악, 속도, 노트 정보 등 스테이지 정보를 파싱하고 세팅한다.
        NoteDataCreate();
        // 게임을 시작한다.
        StartCoroutine(GameStart());

        noteList[0].myTurn = true;
    }

    void FixedUpdate()
    {
        if (gameStatus == GameState.Playing)
        {
            playTime += Time.fixedDeltaTime;
        }
    }

    void Update()
    {
        // 게임이 플레이 중일 때만 작동한다.
        if (gameStatus.Equals(GameState.Playing))
        {
            if (characterController.warningFlg)
            {
                warningObj.SetActive(true);
            }
            else
            {
                warningObj.SetActive(false);
            }
        }
    }

    private IEnumerator WaitLoading()
    {
        while (SceneLoadManager.Instance.isLoading)
        {
            yield return null;
        }
    }

    public void NextNodeTurn()
    {
        noteIndex++;
        if (noteIndex >= noteList.Count) noteIndex = 0;
        noteList[noteIndex].myTurn = true;
    }

    /// <summary>
    /// 인덱스 노드까지의 누적 거리를 구한다.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>float</returns>
    public float GetSumDistance(int index)
    {
        float result = 0;
        for (int i = 0; i < index; i++)
        {
            result += Vector2.Distance(railNodes[i], railNodes[i + 1]);
        }
        return result;
    }

    public float GetNodeTime(int index)
    {
        float result = 0;
        for (int i = 0; i < index; i++)
        {
            result += Vector2.Distance(railNodes[i], railNodes[i + 1]);
        }
        return result / StageData.speedMultiplier / StageConstant.DEFAULT_SPEED;
    }

    public void NoteDataCreate()
    {
        // 노트 데이터를 파싱한다.
        FindObjectOfType<JudgeCircleManager>().CreateCirclePool(StageData.noteList);
    }

    private IEnumerator GameStart()
    {
        JudgeTrailerController judgeTrailerController = FindObjectOfType<JudgeTrailerController>();
        nowHP = maxHP;

        yield return StartCoroutine(WaitLoading());
        yield return FindObjectOfType<GameStatusNoticeManager>().GameStart().WaitForCompletion();
        FindObjectOfType<CharacterSpriteController>().SetCharacterStatus(CharacterSpriteStatus.Fly);
        judgeTrailerController.MoveToNextNode();
    }

    public void JudgeDataAdd(JudgeResult judgeResultUnit)
    {
        switch (judgeResultUnit)
        {
            case JudgeResult.Perfect:
                judgeInfoData.perfect++;
                break;
            case JudgeResult.Good:
                judgeInfoData.good++;
                break;
            case JudgeResult.Bad:
                judgeInfoData.bad++;
                break;
            case JudgeResult.Miss:
                judgeInfoData.miss++;
                break;
        }
    }

    public enum GameState
    {
        Ready,
        Playing,
        End
    }
}

public enum JudgeResult
{
    Perfect,
    Good,
    Bad,
    Miss
}
