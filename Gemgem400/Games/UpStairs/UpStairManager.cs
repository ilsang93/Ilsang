using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using GemgemAr;
using System;

/// <summary>
/// 미니 게임 "UpStair"를 관리하는 매니저 클래스
/// </summary>
public class UpStairManager : MonoBehaviour
{

    [System.Serializable]
    public struct Difficulty
    {
        [Header("목표 계단 수")] [Range(30, 500)] public int targetHeight;
    }
    [Header("이벤트 데이터 사용 여부")] public bool useStairEvent;
    public enum UpStairState
    {
        Ready,
        Play,
        Wait,
        End
    }

    public Sprite BackgroundSprite
    {
        set
        {
            _backGround.ChangeImage(value);
        }
    }
    [Header("테스트할 스테이지 번호")] public int testStageNumber;
    [SerializeField] private UpStairsPlayer _player;
    [SerializeField] private UpStairsEventManager _eventManager;
    [SerializeField] private BackGroundSizeInit _backGround;
    public List<Difficulty> difficultyList;
    [HideInInspector] public Difficulty nowDifficulty;
    private UpStairState _gameState = UpStairState.Ready;
    public UpStairState GameState
    {
        get => _gameState;    
        set
        {
            _gameState = value;
        }
    }

    public int Height
    {
        get
        {
            if (_player.playerJumper.CurStair != null)
            {
                return _player.playerJumper.CurStair.height;
            }
            else
            {
                return 0;
            }
        }
    }

    private UpStairsStairManager _upStairsStairManager;
    private int targetHeight;

    public int TargetHeight
    {
        get
        {
            // 이벤트 데이터 사용할 경우
            if (useStairEvent == true && _eventManager.IsEventDataLoaded)
            {
                return _eventManager.StairCnt;
            }
            else
            {
                return nowDifficulty.targetHeight;
            }
        }
    }

    public event Action onEndGame;
    
    public ScoreController scoreController;
    

    private void Start()
    {
        // AudioManager.Instance.PlayBgm(Bgms.SkydivingLoopable);
        _upStairsStairManager = FindObjectOfType<UpStairsStairManager>();
        scoreController = CommonGameManager.Instance.InstantiateScoreController(this.transform);

        if (CommonGameManager.Instance.GameDifficulty > difficultyList.Count - 1)
        {
            nowDifficulty = difficultyList[difficultyList.Count - 1];
        }
        else
        {
            nowDifficulty = difficultyList[CommonGameManager.Instance.GameDifficulty];
        }
        scoreController.SetActiveCountDown(true);
        scoreController.InitScore();
        //scoreController.ComboActive(new Vector3(0f, 300f, 0f), true);
        CommonGameManager.Instance.HtpStart();
        StartCoroutine(CR_StartGame());
        
    }

    // Update is called once per frame
    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Alpha0))
            GameClear();
#endif
        if (!CommonGameManager.Instance.htpEndFlg)
        {
            return;
        }

        if (GameState == UpStairState.End)
        {
            return;
        }

        if (scoreController.countdownFinished)
        {
            GameFail();
            return;
        }

        if (GameState == UpStairState.Play && Height < TargetHeight)
        {
            _player.GetInput(NetworkManager.inputFlg);
        }
    }

    private IEnumerator CR_StartGame()
    {
        while (CommonGameManager.Instance.htpEndFlg == false)
        {
            yield return null;
        }
        GameState = UpStairState.Play;
        CommonGameManager.Instance.SetOnSkipGame(() =>
        {
            GameState = UpStairState.End;
        });
        Debug.Log(TargetHeight + "타겟 목표");
        _player.playerJumper.onChangeStair += () =>
        {
            // 이벤트 사용
            if (useStairEvent)
            {
                
                //마지막 층일 경우
                if (Height >= TargetHeight)
                {
                    _eventManager.FloorEventRun(Height, onEndGame);
                }
                else
                {
                    _eventManager.FloorEventRun(Height);
                }
            }
            else
            {
                if (Height >= TargetHeight)
                {
                    onEndGame?.Invoke();
                }
            }
        };
        _upStairsStairManager.Init();
        FindObjectOfType<UpStairsProgressManager>().TargetHeight = TargetHeight;
    }
    
    public void GameClear()
    {
        GameState = UpStairState.End;
        AudioManager.Instance.StopBgms(() =>
        {
            CommonGameManager.Instance.EgStart();
        });
        
    }

    private void GameFail()
    {
        GameState = UpStairState.End;
        CommonGameManager.Instance.FgStart();
        return;
    }
}