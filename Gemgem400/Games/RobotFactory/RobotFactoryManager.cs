using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace GemgemAr
{
    /// <summary>
    /// 미니 게임 "로봇 공장"을 제어하는 매니저 클래스
    /// </summary>
    public class RobotFactoryManager : MonoBehaviour
    {
        [SerializeField] private GameObject robotFaceOrigin;
        [SerializeField] private GameObject leftScrewOrigin;
        [SerializeField] private GameObject rightScrewOrigin;

        private int correctCnt;
        private string gameStatus = "Ready";
        private Vector3 leftScrewPreparedPos, rightScrewPreparedPos;

        private GameObject robotFaceNow, leftScrewNow, rightScrewNow;
        private Vector3 robotFaceTargetPos, leftScrewTargetPos, rightScrewTargetPos;

        public List<Difficulty> difficultyList;

        [HideInInspector]
        public Difficulty nowDifficulty;

        [System.SerializableAttribute]
        public struct Difficulty
        {
            [Space(20f)]
            [Header("나사 준비 상태 이동 속도")]
            [Range(5, 20)]
            [SerializeField]
            public float screwPrepareSpeed;

            [Header("나사 발사 속도")]
            [Range(40, 80)]
            [SerializeField]
            public float screwSpeed;

            [Header("최소 얼굴 속도")]
            [Range(1, 5)]
            [SerializeField]
            public float minFaceSpeed;

            [Header("최대 얼굴 속도")]
            [Range(3, 10)]
            [SerializeField]
            public float maxFaceSpeed;

            [Header("목표 로봇 조립 수")]
            [Range(5, 20)]
            [SerializeField]
            public int targetCorrectCnt;

            [Header("시간 제한")]
            public float countdown;
        }

        public ScoreController scoreController;
        // Start is called before the first frame update
        private void Start()
        {
            scoreController = CommonGameManager.Instance.InstantiateScoreController(this.transform);

            AudioManager.Instance.PlayBgm(Bgms.RedNBlueLoopable);
            if (CommonGameManager.Instance.GameDifficulty > difficultyList.Count - 1)
            {
                nowDifficulty = difficultyList[difficultyList.Count - 1];
            }
            else
            {
                nowDifficulty = difficultyList[CommonGameManager.Instance.GameDifficulty];
            }

            scoreController.AddType(ScoreType.Star, nowDifficulty.targetCorrectCnt);
            CommonGameManager.Instance.countdownDuration = nowDifficulty.countdown;
            scoreController.SetActiveCountDown(true);
            scoreController.InitScore();
            scoreController.ComboActive(Vector3.zero, false);

            if (UpStairsSideGameManager.IsLoadedSideGame == false)
            {
                CommonGameManager.Instance.HtpStart();
            }

            leftScrewPreparedPos = new Vector3(-5f, leftScrewOrigin.transform.position.y, 1f);
            rightScrewPreparedPos = new Vector3(5f, rightScrewOrigin.transform.position.y, 1f);

            robotFaceTargetPos = new Vector3(robotFaceOrigin.transform.position.x, -8f, 0f);
            leftScrewTargetPos = new Vector3(-2f, leftScrewOrigin.transform.position.y, 1f);
            rightScrewTargetPos = new Vector3(2f, rightScrewOrigin.transform.position.y, 1f);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!CommonGameManager.Instance.htpEndFlg)
            {
                return;
            }

            if (scoreController.countdownFinished && !gameStatus.Equals("Idle"))
            {
                gameStatus = "Idle";
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

            if (gameStatus.Equals("Idle"))
            {
                return;
            }

            if (gameStatus.Equals("Ready"))
            {
                StartCoroutine(CR_Ready());
            }

            if (gameStatus.Equals("Prepared"))
            {
                StartCoroutine(CR_Prepared());
            }

            if (gameStatus.Equals("Correct"))
            {
                StartCoroutine(CR_Correct());
            }

            if (gameStatus.Equals("Discorrect"))
            {
                StartCoroutine(CR_Discorrect());
            }

            if (gameStatus.Equals("EndGame"))
            {
                gameStatus = "Idle";
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

        private IEnumerator CR_Ready()
        {
            gameStatus = "Idle";

            if (robotFaceNow)
            {
                Destroy(robotFaceNow);
            }

            if (leftScrewNow)
            {
                Destroy(leftScrewNow);
            }

            if (rightScrewNow)
            {
                Destroy(rightScrewNow);
            }

            robotFaceNow = Instantiate(robotFaceOrigin);
            leftScrewNow = Instantiate(leftScrewOrigin);
            rightScrewNow = Instantiate(rightScrewOrigin);

            while (Vector3.Distance(leftScrewPreparedPos, leftScrewNow.transform.position) >= 0.1f || Vector3.Distance(rightScrewPreparedPos, rightScrewNow.transform.position) >= 0.1f)
            {
                leftScrewNow.transform.position = Vector3.MoveTowards(leftScrewNow.transform.position, leftScrewPreparedPos, nowDifficulty.screwPrepareSpeed * Time.deltaTime);
                rightScrewNow.transform.position = Vector3.MoveTowards(rightScrewNow.transform.position, rightScrewPreparedPos, nowDifficulty.screwPrepareSpeed * Time.deltaTime);
                yield return null;
            }

            gameStatus = "Prepared";
        }

        private IEnumerator CR_Prepared()
        {
            gameStatus = "Idle";
            bool inputFlg = false;
            bool correctFlg = false;
            float faceSpeed = Random.Range(nowDifficulty.minFaceSpeed, nowDifficulty.maxFaceSpeed);

            while (Vector3.Distance(robotFaceNow.transform.position, robotFaceTargetPos) >= 0.1f)
            {
                if (!correctFlg)
                {
                    robotFaceNow.transform.position = Vector3.MoveTowards(robotFaceNow.transform.position, robotFaceTargetPos, faceSpeed * Time.deltaTime);
                }

                if (NetworkManager.inputFlg && !inputFlg && robotFaceNow.GetComponent<RobotFaceManager>().canInputFlg)
                {
                    inputFlg = true;

                    if (robotFaceNow.GetComponent<RobotFaceManager>().correctFlg)
                    {
                        correctCnt++;
                        correctFlg = true;
                    }
                }

                if (inputFlg)
                {
                    leftScrewNow.transform.position = Vector3.MoveTowards(leftScrewNow.transform.position, leftScrewTargetPos, nowDifficulty.screwSpeed * Time.deltaTime);
                    rightScrewNow.transform.position = Vector3.MoveTowards(rightScrewNow.transform.position, rightScrewTargetPos, nowDifficulty.screwSpeed * Time.deltaTime);
                }

                if (correctFlg && Vector3.Distance(leftScrewNow.transform.position, leftScrewTargetPos) <= 0.1f &&
                    Vector3.Distance(rightScrewNow.transform.position, rightScrewTargetPos) <= 0.1f)
                {
                    break;
                }

                yield return null;
            }

            if (correctFlg)
            {
                AudioManager.Instance.PlaySfx(Sfxs.Buff2);
                AudioManager.Instance.PlaySfx(Sfxs.SciFiBeamReload1);
                gameStatus = "Correct";
            }
            else
            {
                AudioManager.Instance.PlaySfx(Sfxs.GameOver);
                gameStatus = "Discorrect";
            }
        }

        private IEnumerator CR_Correct()
        {
            gameStatus = "Idle";

            robotFaceNow.transform.position = Vector3.zero;
            leftScrewNow.transform.position = leftScrewTargetPos;
            rightScrewNow.transform.position = rightScrewTargetPos;

            leftScrewNow.transform.SetParent(robotFaceNow.transform);
            rightScrewNow.transform.SetParent(robotFaceNow.transform);

            scoreController.AddPointEach(robotFaceNow.transform.position, ScoreType.Star);
            scoreController.ComboAdd();

            robotFaceNow.GetComponent<RobotFaceManager>().ChangeFaceSpriteToCorrect();
            Vector3 finishTargetPos = new(Random.Range(0, 2) == 0 ? -8f : 8f, Random.Range(0, 2) == 0
                    ? robotFaceOrigin.transform.position.y * -1f
                    : robotFaceOrigin.transform.position.y, 0f);

            while (Vector3.Distance(robotFaceNow.transform.position, finishTargetPos) >= 0.1f)
            {
                robotFaceNow.transform.position = Vector3.MoveTowards(robotFaceNow.transform.position, finishTargetPos, 5f * Time.deltaTime);
                yield return null;
            }

            if (nowDifficulty.targetCorrectCnt <= correctCnt)
            {
                gameStatus = "EndGame";
            }
            else
            {
                gameStatus = "Ready";
            }
        }

        private IEnumerator CR_Discorrect()
        {
            scoreController.ComboInit();
            gameStatus = "Idle";
            yield return new WaitForSeconds(2f);
            gameStatus = "Ready";
        }
    }
}