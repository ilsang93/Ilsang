using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance { get; private set; }

    [SerializeField] private JudgeInfoController judgeInfoController;
    [SerializeField] private GradeController gradeController;
    [SerializeField] private GameObject deskObj;
    [SerializeField] private Button nextButton;

    private readonly Vector2 RESULT_PANEL_DISABLED_POS = new(2000, 0);
    private readonly Vector2 RESULT_PANEL_ENABLED_POS = Vector2.zero;
    private CanvasGroup canvasGroup;
    private Coroutine jiAnimCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitResultPanel()
    {
        GetComponent<RectTransform>().anchoredPosition = RESULT_PANEL_DISABLED_POS;
    }

    public void ShowResultPanel(JudgeInfoData data)
    {
        // 결과 데이터를 세팅한다
        judgeInfoController.SetInfo(data);
        gradeController.SetGrade(data.score);

        // 화면을 가린다.

        // 결과 패널을 정위치로 이동시킨다.
        GetComponent<RectTransform>().anchoredPosition = RESULT_PANEL_ENABLED_POS;

        Sequence seq = DOTween.Sequence();

        // 책상(백그라운드)를 출현시킨다.
        seq.AppendCallback(() =>
        {
            deskObj.SetActive(true);
        });
        // 캔버스 그룹을 0.5초 흔든다.
        seq.Append(transform.DOShakePosition(0.2f, 10, 10, 90, false, true));
        // 0.2초 대기
        seq.AppendInterval(0.2f);
        // JudgeInfo를 활성화시킨다.
        seq.AppendCallback(() =>
        {
            judgeInfoController.gameObject.SetActive(true);
        });
        seq.Append(transform.DOShakePosition(0.2f, 10, 10, 90, false, true));
        // 0.2초 대기
        seq.AppendCallback(() =>
        {
            jiAnimCoroutine = StartCoroutine(judgeInfoController.DoAnim());
        });
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            gradeController.gameObject.SetActive(true);
        });
        seq.Append(transform.DOShakePosition(0.2f, 10, 10, 90, false, true));
        seq.AppendCallback(() =>
        {
            StartCoroutine(gradeController.DoAnim());
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            nextButton.gameObject.SetActive(true);
        });

        seq.Play();
    }

    private void SkipSequence()
    {
        DOTween.CompleteAll(true);
    }

    private void Retry()
    {
        SceneLoadManager.Instance.ReloadScene();
    }

    private void BackToLobby()
    {
        StartCoroutine(SceneLoadManager.Instance.LoadScene("Lobby"));
    }
}
