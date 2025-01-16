using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace GemgemAr
{
    /// <summary>
    /// 미니게임 "벌룬팝"을 제어하는 매니저 클래스
    /// </summary>
    public class BalloonPopManager : MonoBehaviour
    {
        private const string GAME_PROPERTIES_JSON_PATH = "@@@게임 프로퍼티 경로@@@";

        [SerializeField] private GameObject[] copyTargetObj;
        [SerializeField] private Vector3 minRange;
        [SerializeField] private Vector3 maxRange;
        [SerializeField] private GameObject balloonsObj;


        public List<Difficulty> difficultyList;

        [HideInInspector]
        public Difficulty nowDifficulty;

        [System.SerializableAttribute]
        public struct Difficulty
        {
            [Space(10f)]
            [Header("풍선 갯수")]
            [Range(10, 50)]
            [SerializeField]
            public int balloonCnt;

            [Header("시간 제한")]
            public float countdown;
        }

        private readonly List<GameObject> balloons = new();
        private bool gameSetFlg = true;
        private bool waitFlg;

        public ScoreController scoreController;
        // Start is called before the first frame update
        private void Start()
        {
            scoreController = CommonGameManager.Instance.InstantiateScoreController(transform);
            AudioManager.Instance.PlayBgm(Bgms.SkydivingLoopable);
            if (CommonGameManager.Instance.GameDifficulty > difficultyList.Count - 1)
            {
                nowDifficulty = difficultyList[difficultyList.Count - 1];
            }
            else
            {
                nowDifficulty = difficultyList[CommonGameManager.Instance.GameDifficulty];
            }

            // 보상 이미지를 가리는 10개의 풍선 이상이 설정된 경우 랜덤 위치에 추가한다.
            if (nowDifficulty.balloonCnt > 10)
            {
                for (int i = 0; i < nowDifficulty.balloonCnt - 10; i++)
                {
                    GameObject AddedBalloon = Instantiate(copyTargetObj[i % 4], transform.position, transform.rotation);
                    AddedBalloon.transform.SetParent(balloonsObj.transform);

                    float randomX = Random.Range(minRange.x, maxRange.x);
                    float randomY = Random.Range(minRange.y, maxRange.y);

                    AddedBalloon.transform.localPosition = new Vector3(randomX, randomY, 0);
                }
            }

            Debug.Log("풍선 수 : " + GameObject.FindGameObjectsWithTag("balloon").Length);

            int layer = 0;
            foreach (SpriteRenderer s in balloonsObj.GetComponentsInChildren<SpriteRenderer>())
            {
                Debug.Log("풍선이름 : " + s.gameObject.name);
                balloons.Add(s.gameObject);
                s.sortingOrder = layer;
                layer++;
            }

            CommonGameManager.Instance.countdownDuration = nowDifficulty.countdown;
            scoreController.SetActiveCountDown(true);

            if(UpStairsSideGameManager.IsLoadedSideGame == false)
                CommonGameManager.Instance.HtpStart();

        }

        // Update is called once per frame
        private void Update()
        {
            if (waitFlg)
            {
                return;
            }

            if (NetworkManager.inputFlg && CommonGameManager.Instance.htpEndFlg && gameSetFlg)
            {
                waitFlg = true;

                GameObject balloon = balloons[balloons.Count - 1];

                balloon.GetComponent<BalloonPopBalloon>().Pop();

                balloons.RemoveAt(balloons.Count - 1);

                StartCoroutine(delay());
            }

            if ((balloons.Count == 0 || scoreController.countdownFinished) && gameSetFlg)
            {
                gameSetFlg = false;
                //CommonGameManager.Instance.htpEndFlg = false;
                Debug.Log("게임 끝");
                AudioManager.Instance.StopBgms(() =>
                {
                    if (UpStairsSideGameManager.IsLoadedSideGame)
                    {
                        UpStairsSideGameManager.Instacne.EndSideGame();    
                    }
                    else
                    {
                        CommonGameManager.Instance.EgStart();    
                    }
                });
            }
        }

        private IEnumerator delay()
        {
            yield return new WaitForSeconds(0.3f);
            waitFlg = false;
        }
    }
}