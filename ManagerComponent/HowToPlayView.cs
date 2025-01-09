using DG.Tweening;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using static GemgemAr.CustomAnalyticsService;

namespace GemgemAr
{
    /// <summary>
    /// 미니 게임 시작 전, 해야할 재활 동작에 대한 설명과 동영상 출력을 제어하는 클래스
    /// </summary>
    public class HowToPlayView : MonoBehaviour
    {
        [SerializeField] private RectTransform startTextRect;
        [SerializeField] private RectTransform htpRect;

        [SerializeField] private RectTransform realMotionRect;

        // Start is called before the first frame update
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Text followText;
        [SerializeField] private Text gestureText;
        [SerializeField] private Text stageText;
        [SerializeField] private Button QuitBtn;
        [SerializeField] private Button SkipBtn;

        private bool skipFlg;
        //[SerializeField] private Button SkipBtn;
        public string url { get; set; }
        //private bool skipFlg;
        private bool htpInputFlg = false;
        private bool videoEndFlg = false;

        void Start()
        {
            //skipFlg = false;
            QuitBtn.onClick.AddListener(() =>
            {
                LogCustomEvent(CustomAnalyticsEvent.totalPlayTime, 0, "@@@@@@@");
                Loading.SetTargetScene("@@@레벨 선택 화면@@@");
                Loading.Instance.LoadScene();
            });
            SkipBtn.onClick.AddListener(() =>
            {
                skipFlg = true;
            });
        }

        public void OnShow()
        {
            videoEndFlg = false;
            StartCoroutine(CRHowToPlay());
        }

        private IEnumerator CRHowToPlay()
        {
            stageText.text = $"STAGE {PlayingData.Instance.selectedStage}";
            gestureText.text = (PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand.Equals("right") ? "오른손" : "왼손") + " : "
                + PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion].gestureKoName;


            CommonGameManager.Instance.NetworkStart();

            htpRect.localScale = Vector2.zero;

            if (!PlayerPrefs.HasKey("@@@재도전 플래그@@@"))
            {
                CommonGameManager.Instance.ScaleRect(htpRect, Vector2.zero, Vector2.one, 0.5f);
                yield return new WaitForSeconds(0.5f);
                if (!CommonGameManager.Instance.TestModeFlg)
                {
                    yield return StartCoroutine(CRYouTubePlay());
                }
                CommonGameManager.Instance.htpEndFlg = true;
                yield return new WaitForSeconds(0.5f);
                CommonGameManager.Instance.ScaleRect(htpRect, Vector2.one, Vector2.zero, 0.5f);
                // 12번째 레이어를 활성화 한다. 기본에 활성화 되어있는 값은 유지한다.
                var tempLayers = CommonGameManager.Instance.overlayCamera.cullingMask;
                print("변경 전 : " + tempLayers.ToString());
                CommonGameManager.Instance.overlayCamera.cullingMask = tempLayers | (1 << 12);
                print("변경 후 : " + CommonGameManager.Instance.overlayCamera.cullingMask.ToString());
                yield return new WaitForSeconds(0.5f);
                gameObject.SetActive(false);
            }
            else
            {
                PlayerPrefs.DeleteKey("RetryFlg");
                gameObject.SetActive(false);
                CommonGameManager.Instance.htpEndFlg = true;
                // 12번째 레이어를 활성화 한다. 기본에 활성화 되어있는 값은 유지한다.
                var tempLayers = CommonGameManager.Instance.overlayCamera.cullingMask;
                print("변경 전 : " + tempLayers.ToString());
                CommonGameManager.Instance.overlayCamera.cullingMask = tempLayers | (1 << 12);
                print("변경 후 : " + CommonGameManager.Instance.overlayCamera.cullingMask.ToString());
            }
        }

        private IEnumerator CRYouTubePlay()
        {
            Debug.Log("영상재생 : " + PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion].gameVideo);


            Debug.Log("손 방향 : " + PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand);
            if (flipList.Contains(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion))
            {
                if ("right".Equals(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand))
                {
                    realMotionRect.localScale = new Vector3(-1.25f, 1.01f, 1);
                }
                else
                {
                    realMotionRect.localScale = new Vector3(1.25f, 1.01f, 1);
                }
            }
            else
            {
                if ("right".Equals(PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingHand))
                {
                    realMotionRect.localScale = new Vector3(1.25f, 1.01f, 1);
                }
                else
                {
                    realMotionRect.localScale = new Vector3(-1.25f, 1.01f, 1);
                }
            }

            videoPlayer.clip = Resources.Load<VideoClip>("@@@경로@@@" + PlayingData.Instance.motionData[PlayingData.Instance.eachDataList[PlayingData.Instance.playingStep].playingMotion].gameVideo);
            videoPlayer.Play();

            videoPlayer.loopPointReached += VideoPlayer_loopPointReached;

            followText.transform.DOScale(Vector3.one * 1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
            videoPlayer.SetDirectAudioVolume(0, 0.0f);

            while (true)
            {
                if (NetworkManager.inputFlg || skipFlg)
                {
                    htpInputFlg = true;
                }

                if (htpInputFlg)
                {
                    followText.text = "곧 게임이 시작돼요!";
                }

                yield return null;

                if (videoEndFlg && htpInputFlg)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 영상이 반복 지점(모든 분량이 재생된)에 도달한 경우 실행되는 메서드
        /// </summary>
        /// <param name="source"></param>
        private void VideoPlayer_loopPointReached(VideoPlayer source)
        {
            if (htpInputFlg)
            {
                videoEndFlg = true;
                videoPlayer.Stop();
                videoPlayer.targetTexture.Release();
                CommonGameManager.Instance.StartHandTrackingSolution();
            }
        }

        /// <summary>
        /// 뒤집어서 출력해야 하는 영상 리스트
        /// </summary>
        public static readonly List<string> flipList = new List<string> {
            "@@@동작 ID 1@@@",
            "@@@동작 ID 2@@@",
            "@@@동작 ID 3@@@",
            "@@@동작 ID 4@@@"
        };
    }
}