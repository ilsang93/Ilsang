
using GemgemAr;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

/// <summary>
/// 사전 데이터로부터 각 계단 층계에 이벤트를 설정하고, 차례가 되었을 때 이벤트를 실행을 관리하는 매니저 클래스
/// </summary>
public class UpStairsEventManager : MonoBehaviour
{
    public bool IsEventDataLoaded
    {
        get;
        set;
    }
    public int StairCnt
    {
        get => _levelData.stairCnt;
    }
    public bool IsEventRunning
    {
        get => isEventRunning;
        set
        {
            isEventRunning = value;

            if (_upStairManager.GameState == UpStairManager.UpStairState.End)
                return;

            if (isEventRunning)
            {
                _upStairManager.GameState = UpStairManager.UpStairState.Wait;
            }
            else
            {
                _upStairManager.GameState = UpStairManager.UpStairState.Play;
            }
        }
    }
    [SerializeField] private UpStairManager _upStairManager;
    [SerializeField] private UpStairsStairManager _upStairsStairManager;
    [SerializeField] private UpStairsMiniGameBattle battleMiniGamePrefab;
    private event Action onEventEnd;
    private UpstairLevelData _levelData;
    private StairElement _stairElement;
    private UpStairEggPieceManager _eggPieceManager;
    private Dictionary<int, Queue<FloorElement>> stageData = new Dictionary<int, Queue<FloorElement>>();
    private Dictionary<int, GameObject> eventObjectData = new Dictionary<int, GameObject>();
    private Queue<FloorElement> _curQueue;
    private AlertController alertController;
    private bool isEventRunning;
    private bool isWaitEvent;
    private GameObject curEventObjectInstance;
    private UpstairSlideAlretController slideAlretController;

    private void Start()
    {
        _eggPieceManager = FindObjectOfType<UpStairEggPieceManager>();
        alertController = FindObjectOfType<AlertController>();
        slideAlretController = FindObjectOfType<UpstairSlideAlretController>();
    }
    public void FloorEventRun(int floor, Action onEventEnd = null)
    {
        isWaitEvent = false;
        this.onEventEnd = onEventEnd;
        if (stageData.ContainsKey(floor))
        {
            _curQueue = stageData[floor];

            FloorElement tempPeek;
            if (_curQueue.TryPeek(out tempPeek))
            {
                if (_curQueue.Count == 0)
                    return;


                // 대기시간을 스킵해야 하는 이벤트인지 아닌지 체크
                foreach (var peekedElement in _curQueue)
                {
                    switch (peekedElement.type)
                    {
                        case FloorEventType.ALRET:
                        case FloorEventType.SLIDEALRET:
                        case FloorEventType.UPSTAIRS_RESOURCE:
                        case FloorEventType.EARNEGG:
                            break;
                        default:
                            isWaitEvent = true;
                            break;
                    }

                    if (isWaitEvent == true)
                        break;
                }

                // 이벤트가 특정 이벤트 딱 하나일경우 대기 X
                if (isWaitEvent == false)
                {
                    FloorElementRecursionDeque();
                }
                else
                {
                    IsEventRunning = true;
                    // 0층이면 바로 시작
                    if (_upStairManager.Height == 0)
                    {
                        FloorElementRecursionDeque();
                    }
                    else
                    {
                        Invoke("FloorElementRecursionDeque", 1.0f);
                    }
                }
            }
        }
        else
        {
            // IsEventRunning = false;
            onEventEnd?.Invoke();
        }

    }

    public GameObject TryGetEventObject(int floor)
    {

        if (eventObjectData.TryGetValue(floor, out GameObject value))
        {
            return value;
        }
        return null;
    }
    public void LoadStageData(int stage)
    {
        Debug.Log(stage + "번 Stage ");
        stageData = new Dictionary<int, Queue<FloorElement>>();
        eventObjectData = new Dictionary<int, GameObject>();
        try
        {
            _levelData = Resources.Load<UpstairLevelData>("@@@UpStair 레벨 데이터@@@" + stage.ToString().PadLeft(3, '0'));

            // stairElements의 갯수만큼 처리를 반복한다.
            for (int i = 0; i < _levelData.stairElements.Count; i++)
            {
                // resultData에 이미 같은 층이 있는 경우 처리를 건너 뛴다.
                if (stageData.ContainsKey(_levelData.stairElements[i].floor))
                {
                    print("에러 : " + _levelData.stairElements[i].floor + "층에 해당하는 중복된 데이터가 있습니다.");
                    print("순서상 먼저 배치된 데이터만 적용됩니다.");
                    continue;
                }
                else
                {
                    Queue<FloorElement> floorQueue = new Queue<FloorElement>();
                    // floorElements의 갯수만큼 처리를 반복한다.
                    for (int j = 0; j < _levelData.stairElements[i].floorElements.Count; j++)
                    {
                        floorQueue.Enqueue(_levelData.stairElements[i].floorElements[j]);
                    }

                    stageData.Add(_levelData.stairElements[i].floor, floorQueue);
                    eventObjectData.Add(_levelData.stairElements[i].floor, _levelData.stairElements[i].EventObject);

                    Debug.Log(_levelData.level);
                }
            }

            IsEventDataLoaded = true;
        }
        catch (Exception e)
        {
            IsEventDataLoaded = false;
            print("이벤트 데이터 로드 실패");
        }
    }

    private IEnumerator AlertProcess(FloorElement element)
    {
        // 알림 이벤트 이전 큐가 실행된 후에 부여할 딜레이
        yield return new WaitForSeconds(element.delayBeforeAlert);

        if (!alertController)
        {
            alertController = FindObjectOfType<AlertController>();
        }

        if (element.alertSound != null)
        {
            alertController.SetAudioClip(element.alertSound);
        }

        if (string.IsNullOrEmpty(name))
        {
            alertController.CallAlert(element.alertMessage, element.alertDuration);
        }
        else if (element.alertSprite == null)
        {
            alertController.CallAlert(element.alertMessage, element.alertDuration, element.alertName);
        }
        else
        {
            alertController.CallAlert(element.alertMessage, element.alertDuration, element.alertName, element.alertSprite);
        }

        // 알림 이벤트 이후 큐가 실행될 때까지 부여할 딜레이.(알림 표시 시점부터 계산. 지속시간과는 관계 없음.)
        yield return new WaitForSeconds(element.delayAfterAlert);
        if (isWaitEvent)
        {
            FloorElementRecursionDeque();
        }
    }

    private IEnumerator CoversationProcess(FloorElement element)
    {
        StaticDialogueManager.CallDialogue(element.dialogueId);
        while (true)
        {
            if (DialogueManager.isConversationActive == false)
                break;
            yield return null;
        }
        FloorElementRecursionDeque();

    }

    private void FloorElementRecursionDeque()
    {
        FloorElement element;
        if (_curQueue.TryDequeue(out element))
        {
            switch (element.type)
            {
                case FloorEventType.NONE:
                    // 무시한다.
                    FloorElementRecursionDeque();
                    break;
                case FloorEventType.ALRET:
                    AlterEvent(element);
                    break;
                case FloorEventType.SLIDEALRET:
                    SlideAlterEvent(element);
                    break;
                case FloorEventType.CONVERSATION:
                    // 대화를 시작한다.
                    ConversationEvent(element);
                    break;
                case FloorEventType.GAME:
                    GameEvent(element);
                    //TODO 미니게임을 실행한다.
                    break;
                case FloorEventType.EFFECT:
                    // 효과 연출을 실행한다. 연출은 지정한 오브젝트에 한하며, 등장 이후 상세 연출은 모듈 외에서 별개로 구현한다.
                    EffectEvent(element);
                    break;
                case FloorEventType.BATTLE:
                    // 전투를 실행한다.
                    BattleEvent(element);
                    break;
                case FloorEventType.EARNEGG:
                    EarnEggEvent();
                    break;
                case FloorEventType.UPSTAIRS_RESOURCE:
                    UpStairsResourceEvent(element);
                    break;
                case FloorEventType.ADD_EVENT_OBJECT:
                    AddEventObjectEvent(element);
                    break;
                case FloorEventType.REMOVE_EVENT_OBJECT:
                    RemoveEventObjectEvent(element);
                    break;
                case FloorEventType.ADD_EVENT_FOLLOWER:
                    AddFollowerObjectEvent(element);
                    break;
                case FloorEventType.REMOVE_EVENT_FOLLOWER:
                    RemoveFollowerObjectEvent(element);
                    break;
            }
        }
        else
        {
            //종료
            if (isWaitEvent)
            {
                IsEventRunning = false;
            }
            onEventEnd?.Invoke();
        }
    }

    // 이벤트 오브젝트 생성, 현재 이벤트 오브젝트가 없을떄만 작동합니다.
    public void AddEventObjectEvent(FloorElement floorElement)
    {
        _upStairsStairManager.MakeObjectOnStair(floorElement.targetHeight, floorElement.eventObject);
        FloorElementRecursionDeque();
    }
    //이벤트 오브젝트 삭제
    public void RemoveEventObjectEvent(FloorElement floorElement)
    {
        _upStairsStairManager.RemoveObjectOnStair(floorElement.targetHeight);
        FloorElementRecursionDeque();
    }
    //팔로워를 추가합니다
    public void AddFollowerObjectEvent(FloorElement floorElement)
    {
        FindObjectOfType<UpStairsPlayer>().AddFollower(floorElement.follower.GetComponent<UpStairsJumper>());
        FloorElementRecursionDeque();
    }
    //팔로워 삭제
    public void RemoveFollowerObjectEvent(FloorElement floorElement)
    {
        FindObjectOfType<UpStairsPlayer>().RemoveFollower(floorElement.follower.name);
        FloorElementRecursionDeque();
    }
    public void UpStairsResourceEvent(FloorElement floorEvent)
    {
        if (floorEvent.upStairsBackgroundSprite != null)
            _upStairManager.BackgroundSprite = floorEvent.upStairsBackgroundSprite;
        if (floorEvent.upStairsStairSprite != null)
            _upStairsStairManager.StairSprite = floorEvent.upStairsStairSprite;
        if (floorEvent.upStairsBgm != null)
        {
            AudioManager.Instance.PlayBgm(floorEvent.upStairsBgm);
        }

        FloorElementRecursionDeque();
    }
    public void EarnEggEvent()
    {
        _eggPieceManager.AddPiece();
        FloorElementRecursionDeque();
    }
    public void AlterEvent(FloorElement floorEvent)
    {
        StartCoroutine(AlertProcess(floorEvent));
    }
    public void SlideAlterEvent(FloorElement floorEvent)
    {
        slideAlretController.ShowSlideAlret(floorEvent.alertMessage, floorEvent.alertSound, floorEvent.alertSprite);
        FloorElementRecursionDeque();
    }
    public void ConversationEvent(FloorElement floorEvent)
    {
        StartCoroutine(CoversationProcess(floorEvent));
    }
    public void GameEvent(FloorElement floorEvent)
    {
        // 게임시작
        if (floorEvent.gameId == "ShiningHand")
        {
            CommonGameManager.Instance.StopHandTrackingSolution();
        }
        UpStairsSideGameManager.Instacne.StartSideGame(floorEvent.gameId, () =>
        {
            FloorElementRecursionDeque();
            if (floorEvent.gameId == "ShiningHand")
            {
                CommonGameManager.Instance.StartHandTrackingSolution();
            }

        });
    }
    public void BattleEvent(FloorElement floorEvent)
    {
        // 배틀시작
        UpStairsMiniGameBattle bt = Instantiate(battleMiniGamePrefab);
        bt.InitBattle(floorEvent.battleObject);
        bt.onMiniGameEnd += () => FloorElementRecursionDeque();
    }
    public void EffectEvent(FloorElement floorEvent)
    {
        // 아직 기획 및 기능 미구현
    }
}