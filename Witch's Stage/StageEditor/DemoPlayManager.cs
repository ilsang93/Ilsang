using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoPlayManager : MonoBehaviour
{
    public static DemoPlayManager Instance { get; private set; }

    [SerializeField] private Button playButton, pauseButton, stopButton;
    [SerializeField] private GameObject judgeTrailer;
    [SerializeField] private GameObject uiMask;
    [SerializeField] private EditorJudgeTrailerController judgeTrailerController;
    [SerializeField] private TextMeshProUGUI demoTimeText;
    private DemoStatus status = DemoStatus.Stop;
    private EditorStatus beforeStatus;
    private float demoTime = 0f;
    public float DemoTime
    {
        get
        {
            return demoTime;
        }
        set
        {
            demoTime = value;

            int minutes = Mathf.FloorToInt(demoTime / 60); // 60초 = 1분
            int seconds = Mathf.FloorToInt(demoTime % 60); // 남은 초
            int milliseconds = Mathf.FloorToInt((demoTime * 1000) % 1000); // 남은 밀리초

            // 00:00:000 형식의 문자열로 변환 (분:초:밀리초)
            demoTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D3}", minutes, seconds, milliseconds);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playButton.onClick.AddListener(Play);
        pauseButton.onClick.AddListener(Pause);
        stopButton.onClick.AddListener(Stop);

        playButton.interactable = true;
        pauseButton.interactable = false;
        stopButton.interactable = false;

        status = DemoStatus.Stop;
        judgeTrailerController.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (DemoStatus.Play.Equals(status) && judgeTrailerController.currentNodeIndex > 0 && judgeTrailerController.gameEnded == false)
        {
            DemoTime += Time.deltaTime;
        }
    }

    public void Play()
    {
        if (!StageEditorManager.Instance.stageMusic)
        {
            CommonMessageManager.Instance.ShowMessage("음악이 선택되지 않았습니다.");
            return;
        }
        if (StageEditorManager.Instance.nodeList.Count <= 1)
        {
            CommonMessageManager.Instance.ShowMessage("경로 노드가 없습니다.");
            return;
        }

        playButton.interactable = false;
        pauseButton.interactable = true;
        stopButton.interactable = true;


        StageEditorManager.Instance.CameraChange(false);
        judgeTrailerController.gameObject.SetActive(true);
        StartCoroutine(PlayDemo());

        beforeStatus = StageEditorManager.Instance.editorStatus;
        StageEditorManager.Instance.OnModeChanged((int)EditorStatus.DemoPlay);
        status = DemoStatus.Play;
    }

    public void Pause()
    {
        if (DemoStatus.Pause.Equals(status))
        {
            playButton.interactable = false;
            pauseButton.interactable = true;
            stopButton.interactable = true;
            StageEditorManager.Instance.audioSource.UnPause();
            status = DemoStatus.Play;
        }
        else
        {
            playButton.interactable = true;
            pauseButton.interactable = true;
            stopButton.interactable = true;
            StageEditorManager.Instance.audioSource.Pause();
            status = DemoStatus.Pause;
        }
        judgeTrailerController.gamePaused = !judgeTrailerController.gamePaused;
    }

    public void Stop()
    {
        playButton.interactable = true;
        pauseButton.interactable = false;
        stopButton.interactable = false;

        judgeTrailerController.ResetPosition();
        judgeTrailerController.gameObject.SetActive(false);
        StageEditorManager.Instance.CameraChange(true);
        StageEditorManager.Instance.audioSource.Stop();

        StageEditorManager.Instance.OnModeChanged((int)beforeStatus);

        DemoTime = 0f;
        status = DemoStatus.Stop;
    }

    private IEnumerator PlayDemo()
    {
        yield return new WaitForSeconds(1.5f);
        judgeTrailerController.MoveToNextNode();
        StageEditorManager.Instance.audioSource.Play();
        yield break;
    }

    private enum DemoStatus
    {
        Play,
        Pause,
        Stop
    }
}
