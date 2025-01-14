using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;

using mixpanel;

using RenderHeads.Media.AVProMovieCapture;

using System;
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;

using static GemgemAr.CustomAnalyticsService;

namespace GemgemAr
{
    /// <summary>
    /// 모든 종류의 미니 게임을 공통으로 관리하기 위한 매니저 클래스
    /// 게임 상태 관리 (시작 전, 진행 중, 일시 정지, 게임 종료)
    /// 카메라 관리
    /// 핸드 트래킹(손 동작 인식) 관리
    /// 테스트 모드 관리
    /// 난이도 관리
    /// 씬 전환 관리
    /// 웹캠 녹화 관리
    /// </summary>
    public class CommonGameManager : MonoBehaviour
    {
        public Camera overlayCamera;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private EndGameView egView;
        [SerializeField] private FailedGameSceneViewController fgView;
        [SerializeField] private HowToPlayView htpView;
        [SerializeField] private HandTrackingSolution handTrackingSolution;
        [SerializeField] private CaptureFromWebCamTexture webCamRecorder;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private ScoreController scoreController;
        [SerializeField] private GameObject networkErrorCanvas;
        [SerializeField] private Button goLevelSelectBtn;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button goNextGame;
        [SerializeField] private RawImage htpCam;
        [SerializeField] private RawImage sideCam;
        [SerializeField] private Text sideStageText;

        [HideInInspector] public bool htpEndFlg;
        [HideInInspector] public bool pauseFlg;
        [HideInInspector] public Vector3 bottomLeft;
        [HideInInspector] public Vector3 topLeft;
        [HideInInspector] public Vector3 bottomRight;
        [HideInInspector] public Vector3 topRight;
        [HideInInspector] public bool videoTransferCompletedFlg;
        [HideInInspector] public Action onSkipGame;

        private bool isPause = false;

        [Space(2f)]

        [Header("테스트 모드 플래그")]
        public bool testModeFlg;
        public bool TestModeFlg
        {
            get
            {
#if UNITY_EDITOR
                return testModeFlg;
#else
                return false;
#endif
            }
            set => testModeFlg = value;
        }

        [Header("난이도 테스트 플래그")]
        [SerializeField]
        private bool difficultyTestFlg;

        [Header("난이도 테스트 시 지정 난이도")]
        [Range(0, 2)]
        [SerializeField]
        private int difficultyTestLevel;

        [Header("카운트 다운 시간(게임 모듈 매니저 사용 권장)")]
        [Range(20, 300)]
        public float countdownDuration = 60f;

        public int GameDifficulty
        {
            get
            {
                if (difficultyTestFlg)
                {
                    return difficultyTestLevel;
                }
                else
                {
                    if (TestModeFlg)
                    {
                        return 1;
                    }
                    else
                    {
                        return PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].difficulty;
                    }
                }
            }
        }

        public static CommonGameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (!TestModeFlg)
            {
                sideStageText.text = $"STAGE {PlayingData.Instance.selectedStage} - {PlayingData.Instance.playingStep + 1}";
                networkManager.SetMotion(
                    PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion,
                    PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand
                );
            }
            else
            {
                networkManager.SetMotion(
                    "@@@@@@@",
                    "@@@@@@@"
                );
                Debug.Log("테스트 모드로 기동 중. 서버 접속 및 동작 인식은 기능하지 않습니다.");
            }

            bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
            topLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
            bottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane));
            topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

            goLevelSelectBtn.onClick.AddListener(() =>
            {
                LogCustomEvent(CustomAnalyticsEvent.totalPlayTime, 0, "@@@@@@@");
                isPause = false;
                StopHandTrackingSolution();
                Loading.SetTargetScene("@@@@@@@");
                Loading.Instance.LoadScene();
            });

            goNextGame.onClick.AddListener(() =>
            {
                // 개발용 기능 버튼. 게임을 스킵하고 다음 단계로 이행한다.
                LogCustomEvent(CustomAnalyticsEvent.stepFailed, 0, "@@@@@@@");

                var player = LoginUserModel.Instance.Child;
                var eachGameData = PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep];
                _ = ChildApiService.AddCurrentTherapyResult(networkManager.motionCount, PlayTimeCounter.Instance.PlayStepSec, TherapyResult.skip);
                _ = DiscordWebHook.SendMessageAsync($"레벨 스킵/{player?.Name ?? ""}/{player?.Id ?? ""}/스테이지: {player?.NextUnlockableStage ?? -1}");

                isPause = false;
                StopHandTrackingSolution();
                networkManager.Disconnect();

                onSkipGame?.Invoke();
                onSkipGame = null;

                Loading.SetTargetScene("@@@@@@@");
                Loading.Instance.LoadScene();
            });

            pauseBtn.onClick.AddListener(() =>
            {
                isPause = !isPause;
            });

            InitializeWebCamRecorder();
            LogCustomEvent(CustomAnalyticsEvent.stepPlaying, 0);
        }

        public void SetOnSkipGame(Action action)
        {
            onSkipGame = action;
        }

        void InitializeWebCamRecorder()
        {
            if (webCamRecorder)
            {
                OnStartHandTrackingSolutionEvent += OnStartHandTrackingSolution;
                OnStopHandTrackingSolutionEvent += OnStopHandTrackingSolution;

                webCamRecorder.CompletedFileWritingAction += OnCompletedFileWriting;
            }
        }

        private void Update()
        {
            if (isPause)
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
            }
            else
            {
                Time.timeScale = 1;
                pausePanel.SetActive(false);
            }

            if (TestModeFlg)
            {
                return;
            }

            if (!htpCam.texture && !htpEndFlg)
            {
                htpCam.texture = sideCam.texture;
            }

#if UNITY_ANDROID
            htpCam.transform.rotation = sideCam.transform.rotation;
#elif UNITY_IOS
            if (sideCam.transform.rotation.w == 1f) {
                htpCam.transform.rotation = Quaternion.Euler(sideCam.transform.rotation.x + 180f, sideCam.transform.rotation.y, sideCam.transform.rotation.z);
            } else {
                htpCam.transform.rotation = Quaternion.Euler(sideCam.transform.rotation.x, sideCam.transform.rotation.y + 180f, sideCam.transform.rotation.z);
            }
#endif

            if (!networkManager.connectedFlg)
            {
                if (!networkErrorCanvas.activeInHierarchy)
                {
                    networkErrorCanvas.SetActive(true);
                    pauseFlg = true;
                }
            }
            else if (networkErrorCanvas.activeInHierarchy)
            {
                networkErrorCanvas.SetActive(false);
                pauseFlg = false;
            }
        }

        private void OnDestroy()
        {
            StopHandTrackingSolution();
            OnStartHandTrackingSolutionEvent -= OnStartHandTrackingSolution;
            OnStopHandTrackingSolutionEvent -= OnStopHandTrackingSolution;
        }

        public event Action OnStartHandTrackingSolutionEvent;
        public event Action OnStopHandTrackingSolutionEvent;

        private void OnCompletedFileWriting(FileWritingHandler handler)
        {
            if (null == handler)
            {
                Debug.LogError("null == handler");
                return;
            }

            if (!handler.IsFileReady())
            {
                Debug.LogError($"녹화실패, {handler.Path}");
            }
        }

        private void OnStartHandTrackingSolution()
        {
            StartRecord();
        }

        async void StartRecord()
        {
            if (ImageSourceProvider.ImageSource is WebCamSource webCamSource)
            {
                // 10 secs
                while (!webCamSource.isPlaying)
                {
                    await Task.Delay(1000);
                }

                if (webCamRecorder)
                {
                    WebCamTexture targetWebCamTexture = webCamSource.GetCurrentTexture() as WebCamTexture;
                    if (!targetWebCamTexture || targetWebCamTexture.width <= 16 || targetWebCamTexture.height <= 16)
                    {
                        Debug.LogError("targetWebCamTexture is null or invalid resolution");
                        return;
                    }

                    webCamRecorder.SetSourceTexture(targetWebCamTexture);
#if UNITY_IOS
                    //좌우가 반전되는 버그가 있음. 플러그인 제작자가 수정할때까지 임시로 해결
                    //https://github.com/RenderHeads/UnityPlugin-AVProMovieCapture/issues/382
                    //webCamRecorder.FlipVertically = true;
#endif

                    var stage = sideStageText != null ? sideStageText.text : "STAGE0";
                    var name = "NULL";
                    var playingMotion = "error";
                    var playingHand = "error";
                    var size = $"{targetWebCamTexture.width}x{targetWebCamTexture.height}";
                    try
                    {
                        name = LoginUserModel.Instance.Child.Name;
                        playingMotion = PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion;
                        playingHand = PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand;
                    }
                    catch
                    {
                        Debug.LogError("cannot get name, motion, hand");
                    }

                    var fileName = Uploader.RecordFileFormat(name, playingMotion, playingHand, stage, size);
                    webCamRecorder.FilenamePrefix = fileName;
                    webCamRecorder.AppendFilenameTimestamp = false;
                    // 게임당 10분을 넘진 않겠지
                    const float MaxSeconds = 60 * 10;
                    webCamRecorder.StopAfterSecondsElapsed = MaxSeconds;
                    bool started = webCamRecorder.StartCapture();

                    Debug.Log(
                        $"WebCamtexture Name: {targetWebCamTexture?.name}, {targetWebCamTexture?.deviceName}, StartCapture: {started}");

                    _ = DiscordWebHook.SendMessageAsync($"녹화시작_{fileName}, {webCamRecorder.OutputFolderPath}, {webCamRecorder.OutputFolder}, StartCapture: {started}");
                }
            }
        }

        void StopRecord()
        {
            if (webCamRecorder)
            {
                if (webCamRecorder.IsCapturing())
                {
                    webCamRecorder.SetSourceTexture(null);
                    webCamRecorder.StopCapture();
                    Debug.Log($"File Writed : {webCamRecorder.LastFilePath}");
                    _ = DiscordWebHook.SendMessageAsync($"녹화성공_{webCamRecorder.LastFilePath}");
                }
            }
        }

        private void OnStopHandTrackingSolution()
        {
            StopRecord();
        }

        public async void EgStart()
        {
            await ChildApiService.AddCurrentTherapyResult(networkManager.motionCount, PlayTimeCounter.Instance.PlayStepSec, TherapyResult.clear);

            // 게임 스텝 클리어
            LogCustomEvent(CustomAnalyticsEvent.stepPlayTime, networkManager.motionCount, "@@@@@@@");
            LogCustomEvent(CustomAnalyticsEvent.stepClear, networkManager.motionCount);

            PlayTimeCounter.Instance?.StopPlay();

            var user = LoginUserModel.Instance.User;
            var player = LoginUserModel.Instance.Child;
            var therapyDuration = Convert.ToInt32(Math.Round(PlayTimeCounter.Instance.PlayStepSec));
            _ = DiscordWebHook.SendMessageAsync($"{MixpanelAnalyticsService.StageClear} - user: {user.Name}, player: {player.Name}, stage: {PlayingData.Instance.selectedStage}, motionCount: {networkManager.motionCount}, therapyDuration: {therapyDuration}");

            Mixpanel.Track(MixpanelAnalyticsService.StageClear, MixpanelAnalyticsService.GetStageClearData(user, player, PlayingData.Instance.selectedStage, networkManager.motionCount, therapyDuration));

            AudioManager.Instance.PlaySfx(Sfxs.Winsound1);
            StopHandTrackingSolution();
            networkManager.Disconnect();
            // egView 제거
            // egView.gameObject.SetActive(true);
            // egView.SetEndGameText(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand, PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion);
            // egView.OnShow();
            LogCustomEvent(CustomAnalyticsEvent.stepPlayTime, 0, "@@@@@@@");
            Loading.SetTargetScene("@@@@@@@");
            Loading.Instance.LoadScene();
        }

        public void FgStart()
        {
            // 게임 스텝 오버
            LogCustomEvent(CustomAnalyticsEvent.stepPlayTime, networkManager.motionCount, "@@@@@@@");
            LogCustomEvent(CustomAnalyticsEvent.stepFailed, networkManager.motionCount);
            _ = ChildApiService.AddCurrentTherapyResult(networkManager.motionCount, PlayTimeCounter.Instance.PlayStepSec, TherapyResult.over);
            PlayTimeCounter.Instance.StopPlay();
            AudioManager.Instance.PlaySfx(Sfxs.Lostsound1);
            StopHandTrackingSolution();
            networkManager.Disconnect();
            fgView.gameObject.SetActive(true);
            fgView.SetFailedGameText(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand, PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion);
            fgView.OnShow();
        }

        public void HtpStart()
        {
            if (TestModeFlg)
            {
                htpEndFlg = true;
                // 12번째 레이어를 활성화 한다. 기본에 활성화 되어있는 값은 유지한다.
                var tempLayers = overlayCamera.cullingMask;
                print("변경 전 : " + tempLayers.ToString());
                overlayCamera.cullingMask = tempLayers | (1 << 12);
                print("변경 후 : " + overlayCamera.cullingMask.ToString());
                //CommonGameManager.Instance.DoCountDown();
                return;
            }

            if (null == htpView)
            {
                GameObject htpTemp = Resources.Load<GameObject>("@@@@@@@");
                GameObject htpNew = Instantiate(htpTemp);
                htpNew.transform.SetParent(transform);
                htpView = htpNew.GetComponent<HowToPlayView>();
            }
            PlayTimeCounter.Instance.StartPlay();
            htpView.gameObject.SetActive(true);
            htpView.OnShow();
        }

        public void NetworkStart()
        {
            if (!TestModeFlg)
            {
                networkManager.SetGestureTable(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion);
                networkManager.ConnectToServer();
            }
        }

        public async void StartHandTrackingSolution()
        {
            if (!handTrackingSolution)
            {
                Debug.LogError("StartHandTrackingSolution, handTrackingSolution == null");
            }

            Debug.Log($"StartHandTrackingSolution, {SystemInfo.graphicsDeviceType}");

            while (handTrackingSolution.bootstrap == null
                   || false == handTrackingSolution.bootstrap.isFinished)
            {
                await Task.Delay(100);
            }

            // 시작전에는 설정해줘야함
            handTrackingSolution.maxNumHands = 1;
            handTrackingSolution.minDetectionConfidence = 0.7f;
            handTrackingSolution.Play();
            if (ImageSourceProvider.ImageSource)
            {
                ImageSourceProvider.ImageSource.isHorizontallyFlipped = true;
            }

            OnStartHandTrackingSolutionEvent?.Invoke();
        }

        public void StopHandTrackingSolution()
        {
            if (!handTrackingSolution)
            {
                Debug.LogError("StopHandTrackingSolution, handTrackingSolution == null");
                return;
            }

            handTrackingSolution.Stop();
            OnStopHandTrackingSolutionEvent?.Invoke();
        }

        public void ScaleRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
        {
            StartCoroutine(CRScalingRect(rect, startScale, endScale, scalingTime));
        }

        private IEnumerator CRScalingRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime)
        {
            rect.localScale = startScale;
            float t = 0;
            while (t < scalingTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseInOutQuart, t / scalingTime);
                rect.localScale = Vector2.Lerp(startScale, endScale, factor);
                yield return null;
            }
        }

        public ScoreController InstantiateScoreController(Transform parent = null)
        {
            ScoreController sc = Instantiate(scoreController, parent);
            sc.gameObject.SetActive(true);

            return sc;
        }
    }
}