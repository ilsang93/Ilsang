using System;
using GemgemAr;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BascketBallGameManager : MonoBehaviour
{
    [SerializeField] private GameObject ballOriginObj;
    [SerializeField] private GameObject goalObj;
    [SerializeField] private RectTransform goalImage;
    public List<Difficulty> difficultyList;

    [HideInInspector]
    public Difficulty nowDifficulty;

    [SerializableAttribute]
    public struct Difficulty
    {
        [Space(10f)]
        [Header("골대의 속도")]
        [Range(1, 5)]
        public float goalSpeed;

        [Space(10f)]
        [Header("골대의 이동 범위")]
        [Range(1, 5)]
        public float goalRange;

        [Header("공의 속도")]
        [Range(20, 30)]
        public float ballSpeed;

        [Header("목표 골 갯수")]
        [Range(3, 20)]
        public int targetGoalCnt;

        [Header("시간 제한")]
        public float countdown;
    }

    private bool gameStatus = false;
    private GameObject nowBall;
    private int nowGoalCnt;
    private Vector3 readyPosition;
    private Vector3 targetBallScale;
    private bool gameSetFlg = false;

    public ScoreController scoreController;

    private void Start()
    {
        scoreController = CommonGameManager.Instance.InstantiateScoreController(this.transform);
        AudioManager.Instance.PlayBgm(Bgms.GetUpFull);
        if (CommonGameManager.Instance.GameDifficulty > difficultyList.Count - 1)
        {
            nowDifficulty = difficultyList[difficultyList.Count - 1];
        }
        else
        {
            nowDifficulty = difficultyList[CommonGameManager.Instance.GameDifficulty];
        }

        readyPosition = new Vector3(0f, -5f, 0f);
        targetBallScale = ballOriginObj.transform.localScale * 0.4f;

        scoreController.AddType(ScoreType.Star, nowDifficulty.targetGoalCnt);
        CommonGameManager.Instance.countdownDuration = nowDifficulty.countdown;
        scoreController.SetActiveCountDown(true);
        scoreController.InitScore();
        if (UpStairsSideGameManager.IsLoadedSideGame == false)
        {
            CommonGameManager.Instance.HtpStart();
        }
        StartCoroutine(CR_MoveGoal());
    }

    private void Update()
    {
        if (!CommonGameManager.Instance.htpEndFlg)
        {
            return;
        }

        if (scoreController.countdownFinished && !gameSetFlg)
        {
            gameSetFlg = true;
            AudioManager.Instance.StopBgms(() =>
            {
                CommonGameManager.Instance.EgStart();
            });
        }

        if (!gameStatus && !gameSetFlg)
        {
            gameStatus = true;
            StartCoroutine(CR_GoWait());
        }
    }

    private IEnumerator CR_MoveGoal()
    {
        Debug.Log(CommonGameManager.Instance.topRight.x);
        Vector3 targetPosition = new(nowDifficulty.goalRange, goalObj.transform.position.y, 0f);
        while (true)
        {
            if (Vector3.Distance(goalObj.transform.position, targetPosition) < 0.1f)
            {
                targetPosition.x *= -1f;
            }

            goalObj.transform.position = Vector3.MoveTowards(goalObj.transform.position, targetPosition, nowDifficulty.goalSpeed * Time.deltaTime);

            yield return null;

            if (gameStatus.Equals("EndGame"))
            {
                yield break;
            }
        }
    }

    private IEnumerator CR_GoWait()
    {
        nowBall = Instantiate(ballOriginObj);
        nowBall.SetActive(true);

        while (Vector3.Distance(nowBall.transform.position, readyPosition) > 0.1f)
        {
            nowBall.transform.position = Vector3.MoveTowards(nowBall.transform.position, readyPosition, 2 * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(CR_Wait());
    }

    private IEnumerator CR_Wait()
    {
        while (true)
        {
            if (NetworkManager.inputFlg && !gameSetFlg)
            {
                StartCoroutine(CR_Shot());
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator CR_Shot()
    {
        AudioManager.Instance.PlaySfx(Sfxs.IntroLogo_out);
        Rigidbody2D nowRigid = nowBall.GetComponent<Rigidbody2D>();
        SpriteRenderer nowSprite = nowBall.GetComponent<SpriteRenderer>();
        CapsuleCollider2D nowCollider = nowBall.GetComponent<CapsuleCollider2D>();
        BallManager nowBallManager = nowBall.GetComponent<BallManager>();

        float angle = 90f;
        float maxDistance = 10f;

        // 각도를 라디안으로 변환합니다.
        float radAngle = Mathf.Deg2Rad * angle;

        nowRigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 공을 던지는 방향으로 초기 속도를 설정합니다.
        Vector2 throwVelocity = nowDifficulty.ballSpeed * new Vector2(Mathf.Cos(radAngle), Mathf.Sin(radAngle));
        nowRigid.velocity = throwVelocity;

        PhysicsMaterial2D mat = new();
        mat.bounciness = 1f;
        nowRigid.sharedMaterial = mat;

        while (true)
        {
            nowBall.transform.localScale = Vector3.Slerp(nowBall.transform.localScale, targetBallScale, 0.1f * Time.deltaTime * maxDistance);

            Debug.Log(nowRigid.velocity.normalized.y);
            if (nowRigid.velocity.normalized.y < 0)
            {
                nowCollider.isTrigger = false;
                nowSprite.sortingOrder = 2;
                nowBallManager.downFlg = true;

                if (nowBall.transform.position.y < -7f)
                {
                    if (nowBallManager.goalFlg)
                    {
                        Debug.Log("골");
                        StartCoroutine(CR_Goal());
                        yield break;
                    }

                    Debug.Log("실패");
                    StartCoroutine(CR_Miss());
                    yield break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator CR_Goal()
    {
        AudioManager.Instance.PlaySfx(Sfxs.Coin);
        CommonGameManager.Instance.ScaleRect(goalImage, Vector2.zero, Vector2.one * 2, 0.5f);
        yield return new WaitForSeconds(2.5f);
        CommonGameManager.Instance.ScaleRect(goalImage, Vector2.one * 2, Vector2.zero, 0.5f);
        scoreController.AddPointEach(Vector3.zero, ScoreType.Star);
        yield return new WaitForSeconds(1.5f);
        Destroy(nowBall);
        if (++nowGoalCnt >= nowDifficulty.targetGoalCnt)
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
        }
        else
        {
            StartCoroutine(CR_GoWait());
        }
    }

    private IEnumerator CR_Miss()
    {

        AudioManager.Instance.PlaySfx(Sfxs.GameOver);
        Debug.Log("실패");
        Destroy(nowBall);
        StartCoroutine(CR_GoWait());
        yield break;
    }
}