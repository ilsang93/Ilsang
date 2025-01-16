using DG.Tweening;

using GemgemAr;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

/// <summary>
/// 미니 게임 "골키퍼"를 제어하는 매니저 클래스
/// 좌,우 랜덤한 방향으로 날아드는 축구공을 골키퍼를 조작해 막아내는 게임입니다.
/// </summary>
public class GoalKeeperManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject ballOrigin;

    private int blockCount;

    private string gameStatus = "Init";
    private GameObject nowBall;
    private bool playerMoveFlg;

    public List<Difficulty> difficultyList;

    [HideInInspector]
    public Difficulty nowDifficulty;

    [System.SerializableAttribute]
    public struct Difficulty
    {
        [Header("체력")]
        [Range(2, 10)]
        [SerializeField]
        public int maxHp;

        [Header("목표 점수")]
        [Range(3, 20)]
        [SerializeField]
        public int maxScore;

        [Header("공 스피드")]
        [Range(5, 15)]
        [SerializeField]
        public float ballSpeed;

        [Header("시간 제한")]
        public float countdown;
    }

    [SerializeField] private SpriteRenderer goalKeeperSRenderer;
    [SerializeField] private Sprite goalKeeperSpriteA;
    [SerializeField] private Sprite goalKeeperSpriteB;

    [SerializeField] private Transform goalPostTransform;
    [SerializeField] private Transform goalPostBottomTransform;
    private Vector3 ballOriginSize;

    private int score = 0;

    private int ballInitPos = -100;
    private int sameDirCnt;

    private ScoreController scoreController;

    private void Start()
    {
        scoreController = CommonGameManager.Instance.InstantiateScoreController();
        AudioManager.Instance.PlayBgm(Bgms.GetUpFull);
        if (CommonGameManager.Instance.GameDifficulty > difficultyList.Count - 1)
        {
            nowDifficulty = difficultyList[difficultyList.Count - 1];
        }
        else
        {
            nowDifficulty = difficultyList[CommonGameManager.Instance.GameDifficulty];
        }
        ballOriginSize = ballOrigin.transform.localScale;
        scoreController.AddType(ScoreType.Star, nowDifficulty.maxScore);
        CommonGameManager.Instance.countdownDuration = nowDifficulty.countdown;
        scoreController.SetActiveCountDown(true);
        scoreController.InitScore();
        scoreController.ComboActive(Vector3.zero, true);
        if (UpStairsSideGameManager.IsLoadedSideGame == false)
            CommonGameManager.Instance.HtpStart();

        CommonGameManager.Instance.SetOnSkipGame(() =>
        {
            gameStatus = "Idle";
        });
    }

    private void Update()
    {
        if (gameStatus.Equals("Init"))
        {
            StartCoroutine(CR_Init());
            return;
        }

        if (!CommonGameManager.Instance.htpEndFlg)
        {
            return;
        }

        if (NetworkManager.inputFlg && !playerMoveFlg)
        {
            AudioManager.Instance.PlaySfx(Sfxs.IntroLogo_out);
            playerMoveFlg = true;

            float targetJumpPos = playerObj.transform.position.x * -1f;

            int spriteDir = 0;
            if (targetJumpPos > playerObj.transform.position.x)
            {
                spriteDir = -1;
            }
            else
            {
                spriteDir = 1;
            }

            playerObj.transform.localScale = new Vector3(Mathf.Abs(playerObj.transform.localScale.x) * spriteDir, playerObj.transform.localScale.y, playerObj.transform.localScale.z);
            goalKeeperSRenderer.sprite = goalKeeperSpriteB;

            playerObj.transform.DOJump(new Vector3(targetJumpPos, playerObj.transform.position.y, playerObj.transform.position.z), 0.5f, 1, 0.5f).OnComplete(() =>
            {
                playerMoveFlg = false;
                goalKeeperSRenderer.sprite = goalKeeperSpriteA;
            });
        }

        switch (gameStatus)
        {
            case "Ready":
                StartCoroutine(CR_Ready());
                break;
            case "Shooting":
                StartCoroutine(CR_Shooting());
                break;
        }

        if (scoreController.countdownFinished)
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

    private IEnumerator CR_Init()
    {
        gameStatus = "Idle";

        int initPos = Random.Range(0, 2) == 0 ? -1 : 1; // true : 왼쪽 | false : 오른쪽

        playerObj.transform.position =
            new Vector3(3f * initPos, playerObj.transform.position.y, playerObj.transform.position.z);

        gameStatus = "Ready";

        yield break;
    }

    private IEnumerator CR_Ready()
    {
        gameStatus = "Idle";
        if (nowDifficulty.maxScore <= blockCount)
        {
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
            yield break;
        }


        int tempBallInitPos = Random.Range(0, 2) == 0 ? -1 : 1; // true : 왼쪽 | false : 오른쪽

        // 같은방향 두번이상 나오지 않도록 하는 처리
        if (ballInitPos == tempBallInitPos)
        {
            sameDirCnt++;
        }
        else
        {
            sameDirCnt = 0;
        }

        if (sameDirCnt >= 2)
        {
            ballInitPos *= -1;
            sameDirCnt = 0;
        }
        else
        {
            ballInitPos = tempBallInitPos;
        }


        nowBall = Instantiate(ballOrigin);

        nowBall.SetActive(true);
        nowBall.transform.DOJump(new Vector3(3f * ballInitPos, -4f, nowBall.transform.position.z), 2f, 1, 1.0f).OnComplete(() =>
        {
            gameStatus = "Shooting";
        });
        nowBall.transform.DOScale(nowBall.transform.localScale * 0.5f, 1.0f);

        yield break;
    }
    private IEnumerator CR_Shooting()
    {
        bool ballHit = false;
        bool ballEnd = false;
        gameStatus = "Idle";

        // 2초 대기한다.
        yield return new WaitForSeconds(1f);

        nowBall.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        AudioManager.Instance.PlaySfx(Sfxs.RandomBoxTap);
        while (true)
        {
            if (ballHit == false)
            {
                nowBall.transform.position = Vector3.MoveTowards(nowBall.transform.position, new Vector3(nowBall.transform.position.x, 10f, nowBall.transform.position.z), nowDifficulty.ballSpeed * Time.deltaTime);
                nowBall.transform.localScale = new Vector3(nowBall.transform.localScale.x - Time.deltaTime, nowBall.transform.localScale.y - Time.deltaTime, nowBall.transform.localScale.z);
            }

            if (nowBall.GetComponent<GoalKeeperBallController>().keepedFlg == true && ballHit == false)
            {
                ballHit = true;
                if (playerMoveFlg == true)
                    nowBall.transform.DOJump(new Vector3(nowBall.transform.position.x * 3, goalPostBottomTransform.position.y, 0f), 4.0f, 1, 1.0f).OnComplete(() => ballEnd = true);
                else
                    nowBall.transform.DOJump(new Vector3(playerObj.transform.position.x, goalPostBottomTransform.position.y * Random.Range(-5f, -7f), 0f), 4.0f, 1, 1.0f).OnComplete(() => ballEnd = true);

                nowBall.transform.DOScale(nowBall.transform.localScale * 2.0f, 1.0f);
            }

            if (nowBall.transform.position.y >= goalPostTransform.position.y && ballHit == false)
            {
                goalPostTransform.DOShakePosition(1, 0.1f);
                Destroy(nowBall.GetComponent<Collider2D>());
                nowBall.GetComponent<SpriteRenderer>().sortingOrder = 0;
                AudioManager.Instance.PlaySfx(Sfxs.RandomBox_Drop);
                ballHit = true;
                nowBall.transform.DOJump(new Vector3(nowBall.transform.position.x, goalPostBottomTransform.position.y, 0f), 1.0f, 1, 0.5f).OnComplete(() =>
                {
                    nowBall.transform.DOJump(new Vector3(nowBall.transform.position.x, goalPostBottomTransform.position.y, 0f), 0.5f, 1, 0.5f).OnComplete(() =>
                    {
                        ballEnd = true;
                    });
                });
            }

            if (ballEnd == true)
            {
                gameStatus = "Ready";
                if (nowBall.GetComponent<GoalKeeperBallController>().keepedFlg)
                {
                    AudioManager.Instance.PlaySfx(Sfxs.Coin);
                    // 성공
                    scoreController.ComboAdd();
                    scoreController.AddPointEach(Vector3.zero, ScoreType.Star);

                    ++blockCount;
                }
                else
                {
                    // 실패
                    AudioManager.Instance.PlaySfx(Sfxs.GameOver);
                    scoreController.ComboInit();
                    if (--nowDifficulty.maxHp <= 0)
                    {
                        AudioManager.Instance.StopBgms(() =>
                        {
                            if (UpStairsSideGameManager.IsLoadedSideGame)
                            {
                                UpStairsSideGameManager.Instacne.EndSideGame();
                            }
                            else
                            {
                                CommonGameManager.Instance.FgStart();
                            }
                        });
                        gameStatus = "Idle";
                    }
                }

                Destroy(nowBall);
                ballHit = false;
                break;
            }

            yield return null;
        }
    }
}