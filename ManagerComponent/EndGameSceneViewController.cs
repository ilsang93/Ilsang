using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static GemgemAr.CustomAnalyticsService;

namespace GemgemAr
{
    /// <summary>
    /// 게임 시퀀스가 모두 종료되고, 게임 플레이 결과를 표시하는 화면을 제어하는 클래스
    /// </summary>
    public class EndGameSceneViewController : MonoBehaviour
    {
        [SerializeField] private Button goLevelSelectBtn;
        [SerializeField] private List<Text> MotionInfoHeaderList;
        [SerializeField] private List<Text> MotionInfoDetailList;
        [SerializeField] private Text StageText;

        // Start is called before the first frame update
        private async void Start()
        {
            StageText.text = $"STAGE{PlayingData.Instance.selectedStage}";

            TodayRewardModel todayReward = await GatchaService.SelectGatchaRemainCount();
            goLevelSelectBtn.onClick.AddListener(() =>
            {
                
                if (todayReward.PackagedRewardCount > 0)
                {
                    Loading.SetTargetScene("@@@뽑기 화면@@@");
                    Loading.Instance.LoadScene();
                }
                else
                {
                    Loading.SetTargetScene("@@@메인 화면@@@");
                    Loading.Instance.LoadScene();    
                }
                
            });

            int PlayedMotionCnt = PlayerPrefs.GetInt("@@@@@@@");

            for (int i = 0; i < PlayedMotionCnt; i++)
            {
                string hand = PlayerPrefs.GetString("@@@@@@@" + (i + 1)).Equals("left") ? "왼손" : "오른손";
                MotionInfoHeaderList[i].text =
                    $"{hand} : {PlayingData.Instance.motionData[PlayerPrefs.GetString("@@@@@@@" + (i + 1))].gestureKoName}";
                MotionInfoDetailList[i].text =
                    $"시행 횟수 : {PlayerPrefs.GetInt(PlayerPrefs.GetString("@@@@@@@" + (i + 1)))}";
            }

            LogCustomEvent(CustomAnalyticsEvent.totalPlayTime, 0, "@@@@@@@");
            LogCustomEvent(CustomAnalyticsEvent.gameAllClear, 0);

            //TODO loginInfo 업데이트 기능이 필요합니다.
            var user = LoginUserModel.Instance.User;
            var child = LoginUserModel.Instance.Child;
            if (user != null && child != null)
            {
                if (PlayingData.Instance.selectedStage == LoginUserModel.Instance.Child.NextUnlockableStage)
                {
                    // 선택한 레벨과 현재 진행 레벨이 같은 경우, 프로그레스를 한 단계 진행시킨다.
                    LoginUserModel.Instance.Child.NextUnlockableStage++;
                }
            }
        }
    }
}