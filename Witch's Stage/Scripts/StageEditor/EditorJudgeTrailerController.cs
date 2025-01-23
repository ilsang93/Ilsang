using UnityEngine;

public class EditorJudgeTrailerController : MonoBehaviour
{
    public static EditorJudgeTrailerController Instance { get; private set; }
    private Vector2 targetPosition;
    private Vector2 startPosition;
    [HideInInspector] public int currentNodeIndex = 0;
    [HideInInspector] public bool gameEnded = false;
    [HideInInspector] public bool gamePaused = false;
    private Vector2 direction;
    private float speed = 0;

    // Update is called once per frame
    void Update()
    {
        if (gameEnded) return;
        if (gamePaused) return;
        if (currentNodeIndex == 0) return;

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            MoveToNextNode();
        }
        else
        {
        }
    }

    public void MoveToNextNode()
    {
        if (StageEditorManager.Instance.nodeList.Count == 0) return;
        if (++currentNodeIndex > StageEditorManager.Instance.nodeList.Count - 1)
        {
            gameEnded = true;
            DemoPlayManager.Instance.Stop();
            return;
        }
        startPosition = targetPosition;
        targetPosition = StageEditorManager.Instance.nodeList[currentNodeIndex].transform.position;
        speed = Vector2.Distance(startPosition, targetPosition) / StageConstant.NODE_TIME;
    }

    public void ResetPosition()
    {
        startPosition = StageEditorManager.Instance.nodeList[0].transform.position;
        targetPosition = StageEditorManager.Instance.nodeList[0].transform.position;
        transform.position = StageEditorManager.Instance.nodeList[0].transform.position;
        currentNodeIndex = 0;
        gameEnded = false;
    }
}
