using UnityEngine;

public class JudgeTrailerController : MonoBehaviour
{
    public static JudgeTrailerController Instance;

    public SpriteRenderer outlineRenderer;

    private Vector2 targetPosition;
    private Vector2 beforePosition;
    private int currentNodeIndex = 0;
    [HideInInspector] public bool gameEnded = false;
    [HideInInspector] public bool gamePaused = false;

    private float playingTime = 0;
    private float nowMoveDistance = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (gameEnded) return;
        if (gamePaused) return;
        if (currentNodeIndex == 0) return;
        if (currentNodeIndex >= StageManager.Instance.railNodes.Count) return;
        
        nowMoveDistance += Time.deltaTime * StageConstant.DEFAULT_SPEED * StageManager.Instance.StageData.speedMultiplier;

        transform.position = Vector2.Lerp(StageManager.Instance.railNodes[currentNodeIndex - 1], StageManager.Instance.railNodes[currentNodeIndex], nowMoveDistance / Vector2.Distance(StageManager.Instance.railNodes[currentNodeIndex - 1], StageManager.Instance.railNodes[currentNodeIndex]));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameEnded) return;
        if (gamePaused) return;
        if (currentNodeIndex == 0) return;

        playingTime += Time.fixedDeltaTime;

        if (Mathf.Abs(playingTime - StageManager.Instance.GetNodeTime(currentNodeIndex)) < 0.01f)
        {
            MoveToNextNode();
            return;
        }
    }

    public void MoveToNextNode()
    {
        if (StageManager.Instance.railNodes.Count == 0) return;

        if (++currentNodeIndex > StageManager.Instance.railNodes.Count - 1)
        {
            // 실제 코드
            StageManager.Instance.gameStatus = StageManager.GameState.End;
            FindObjectOfType<CharacterSpriteController>().SetCharacterStatus(CharacterSpriteStatus.Idle);
            FindAnyObjectByType<GameStatusNoticeManager>().StageClear();
            return;
        }

        nowMoveDistance = 0;

        // 최초 3개 노드는 시작 대기 상태로 한다.
        if (currentNodeIndex == 3)
        {
            StageManager.Instance.audioSource.UnPause();
        }

        beforePosition = StageManager.Instance.railNodes[currentNodeIndex - 1];
        targetPosition = StageManager.Instance.railNodes[currentNodeIndex];
    }
}
