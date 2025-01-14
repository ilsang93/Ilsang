using GemgemAr;

using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine.Purchasing;

/// <summary>
/// 다음 화면을 로드한다. 게임 진행 중에는 단계 진행 연출을 표시한다.
/// </summary>
public class Loading : MonoBehaviour
{
    private static string targetScene = string.Empty;

    private static int selectedLevel;

    public static int SelectedLevel { get => selectedLevel; set => selectedLevel = value; }

    [SerializeField]
    private GameObject HidePanelLeft;
    [SerializeField]
    private GameObject HidePanelRight;
    [SerializeField]
    private Canvas ChangeSeqCanv;
    [SerializeField]
    private GameObject LoadingMaskPanel;

    public static Loading Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Update()
    {
        if (!ChangeSeqCanv.worldCamera)
        {
            if (GameObject.Find("@@@오버레이 카메라@@@"))
            {
                ChangeSeqCanv.worldCamera = GameObject.Find("@@@오버레이 카메라@@@").GetComponent<Camera>();
            }
            else
            {
                ChangeSeqCanv.worldCamera = GameObject.Find("@@@메인 카메라@@@").GetComponent<Camera>();
            }
        }
    }

    public void LoadScene(System.Action OnLoadComplete = null)
    {
        if (targetScene.Equals("Game"))
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (PlayingData.Instance.eachDataList[0].gameType == "@@@준비 운동@@@")
            {
                WarmUpManager.SetVideoAddr(PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[0].playingMotion].beforeVideo);
                WarmUpManager.SetIsLeft(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand.Equals("left"));
                StartCoroutine(LoadingScene("@@@준비 운동@@@"));
            }
            else if (PlayingData.Instance.eachDataList[0].gameType == "@@@정리 운동@@@")
            {
                StretchManager.SetVideoAddr(PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[0].playingMotion].stretchVideo);
                StretchManager.SetIsLeft(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand.Equals("left"));
                StartCoroutine(LoadingScene("@@@정리 운동@@@"));
            }
            else
            {
                StartCoroutine(LoadingScene(PlayingData.Instance.eachDataList[0].gameName));
            }
        }
        else if (targetScene == "@@@다음 게임 씬@@@")
        {
            PlayingData.Instance.SetNextGame();

            if (PlayingData.Instance.playingStep >= PlayingData.Instance.eachDataList.Count)
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                StartCoroutine(LoadingScene("@@@게임 완료 화면@@@"));
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                Debug.Log("이번차례 : " + PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].gameType);

                if (PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].gameType == "@@@준비 운동@@@")
                {
                    WarmUpManager.SetVideoAddr(PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion].beforeVideo);
                    WarmUpManager.SetIsLeft(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand.Equals("left"));
                    StartCoroutine(LoadingScene("@@@준비 운동@@@"));
                }
                else if (PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].gameType == "@@@정리 운동@@@")
                {
                    StretchManager.SetVideoAddr(PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion].stretchVideo);
                    StretchManager.SetIsLeft(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand.Equals("left"));
                    StartCoroutine(LoadingScene("@@@정리 운동@@@"));
                }
                else
                {
                    StartCoroutine(LoadingScene(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].gameName));
                }
            }
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            StartCoroutine(LoadingScene(OnLoadComplete));
        }
    }

    private IEnumerator LoadingScene(System.Action OnLoadComplete = null)
    {
        AsyncOperation asyn = null;
        bool PanelFlg = false;
        HidePanelLeft.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        HidePanelRight.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PanelFlg = true;
            asyn = SceneManager.LoadSceneAsync(targetScene);
        });

        while (asyn == null || !asyn.isDone || !PanelFlg)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.5f);

        HidePanelRight.transform.DOLocalMoveX(960, 0.5f).SetEase(Ease.Linear);
        HidePanelLeft.transform.DOLocalMoveX(-960, 0.5f).SetEase(Ease.Linear);

        OnLoadComplete?.Invoke();
    }

    private IEnumerator LoadingScene(string gameScene)
    {
        AsyncOperation asyn = null;
        bool PanelFlg = false;
        HidePanelLeft.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear);
        HidePanelRight.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PanelFlg = true;
        });

        yield return new WaitForSeconds(2f);
        asyn = SceneManager.LoadSceneAsync(gameScene);
        yield return new WaitForSeconds(1f);

        while (asyn == null || !asyn.isDone || !PanelFlg)
        {
            yield return new WaitForSeconds(0.1f);
        }

        UISFXManager.Instance.AddAllButtonSfx();

        HidePanelRight.transform.DOLocalMoveX(960, 0.5f).SetEase(Ease.Linear);
        HidePanelLeft.transform.DOLocalMoveX(-960, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (gameScene.Equals("@@@게임 완료 화면@@@"))
            {
                UISFXManager.Instance.PlayCommonSfx(CommonSfx.COMPLETE_points_ticker_bonus_score_reward_jingle_01);
            }
        });
    }

    /// <summary>
    ///     Set target scene.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void SetTargetScene(string sceneName, int level)
    {
        targetScene = sceneName;
        SelectedLevel = level;
    }

    public static void SetTargetScene(string sceneName)
    {
        targetScene = sceneName;
    }

    public void ShowLoadingMask()
    {
        LoadingMaskPanel.SetActive(true);
    }

    public void HideLoadingMask()
    {
        LoadingMaskPanel.SetActive(false);
    }
}