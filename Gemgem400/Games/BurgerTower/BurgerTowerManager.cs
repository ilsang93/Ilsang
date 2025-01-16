using GemgemAr;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 미니 게임 "버거 타워"를 관리하는 매니저 클래스
/// 햄버거 재료를 쌓아 햄버거를 완성시키는 게임입니다.
/// </summary>
public class BugerTowerManager : MonoBehaviour
{
    [SerializeField] private GameObject cuttingBoard;
    [SerializeField] private GameObject bugerTop;
    [SerializeField] private List<GameObject> elements;
    [SerializeField] private Button replaceBtn;

    private int bugerCount;
    private int elementCount;
    private GameObject nowCuttingBoard;
    private GameObject nowElement;
    private List<GameObject> nowElements;
    private int sortingOrder;
    private int healthPoint;

    private string statusFlg = "BugerStart";

    public List<Difficulty> difficultyList;

    [HideInInspector]
    public Difficulty nowDifficulty;

    public ScoreController scoreController;
    [System.Serializable]
    public struct Difficulty
    {
        [Space(20f)]
        [Header("재료가 좌우로 움직이는 속도")]
        [Range(2, 10)]
        [SerializeField]
        public float elementSpeed;

        [Header("재료가 움직이는 거리(너무 멀면 버그 가능성)")]
        [Range(1, 8)]
        [SerializeField]
        public float elementMoveDistance;

        [Header("남은 목숨")]
        [Range(3, 10)]
        [SerializeField]
        public int healthPoint;

        [Header("버거 완성 목표 갯수")]
        [Range(3, 20)]
        [SerializeField]
        public int maxBuger;

        [Header("쌓아올릴 재료의 목표 갯수")]
        [Range(3, 8)]
        [SerializeField]
        public int maxBugerElement;

        [Header("시간 제한")]
        public float countdown;
    }

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

        scoreController.AddType(ScoreType.Star, nowDifficulty.maxBuger);
        healthPoint = nowDifficulty.healthPoint;
        CommonGameManager.Instance.countdownDuration = nowDifficulty.countdown;
        scoreController.SetActiveCountDown(true);
        scoreController.InitScore();

        if (UpStairsSideGameManager.IsLoadedSideGame == false)
            CommonGameManager.Instance.HtpStart();
    }

    private void Update()
    {
        if (!CommonGameManager.Instance.htpEndFlg)
        {
            return;
        }

        if (statusFlg.Equals("Idle"))
        {
            // 코루틴을 호출하지 않는 상태
        }
        else if (statusFlg.Equals("BugerStart"))
        {
            bugerCount++;
            nowCuttingBoard = Instantiate(cuttingBoard);
            nowElements = new();
            nowCuttingBoard.SetActive(true);
            StartCoroutine(BugerStart(nowCuttingBoard));
            statusFlg = "Idle";
        }
        else if (statusFlg.Equals("Supply"))
        {
            // 화면 상에 스탠바이 중인 재료가 없는 경우
            int elementConst = Random.Range(0, elements.Count);
            nowElement = Instantiate(elements[elementConst]);

            nowElement.transform.position = new Vector3(0f, 4f, 0f);

            nowElement.SetActive(true);
            statusFlg = "Supplying";
        }
        else if (statusFlg.Equals("Supplying"))
        {
            // 재료가 나오는 처리 실행
            StartCoroutine(BugerSupply(nowElement));
            statusFlg = "Idle";
        }
        else if (statusFlg.Equals("Move"))
        {
            // 화면 상에 재료가 스탠바이 중이고 좌우로 움직이고 있는 경우 경우
            StartCoroutine(BugerMove(nowElement));
            replaceBtn.onClick.AddListener(() =>
            {
                StartCoroutine(BugerReplace());
            });
            statusFlg = "Idle";
        }
        else if (statusFlg.Equals("Drop"))
        {
            // 스탠바이한 재료가 낙하 중이고, 아직 움직임을 멈추지 않은 경우
            StartCoroutine(BugerDrop(nowElement));
            replaceBtn.onClick.RemoveAllListeners();
            statusFlg = "Idle";
        }
        else if (statusFlg.Equals("End"))
        {
            // 낙하한 재료가 도마에서 떨어졌거나, 움직임을 멈춘 경우
            StartCoroutine(BugerEnd());
        }
        else if (statusFlg.Equals("BugerComplete"))
        {
            StartCoroutine(BugerComplete(nowCuttingBoard));
            statusFlg = "Idle";
        }

        if (scoreController.countdownFinished)
        {
            statusFlg = "Idle";
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

    private IEnumerator BugerStart(GameObject cuttingBoardEle)
    {
        Vector3 targetPosition = new(0f, cuttingBoard.transform.position.y, 0f);
        float distance = Vector3.Distance(cuttingBoardEle.transform.position, targetPosition);

        while (distance > 0.01f)
        {
            Vector3 newPosition = Vector3.Lerp(cuttingBoardEle.transform.position, targetPosition,
                nowDifficulty.elementSpeed * Time.deltaTime);
            cuttingBoardEle.transform.position = newPosition;
            distance = Vector3.Distance(cuttingBoardEle.transform.position, targetPosition);
            yield return null;
        }

        sortingOrder = 1;
        statusFlg = "Supply";
    }

    private IEnumerator BugerSupply(GameObject element)
    {
        // 서플라이홀 부터 오브젝트가 내려올 타겟 위치
        Vector3 targetPosition = new(element.transform.position.x, 1f, 0f);
        float distance = Vector3.Distance(element.transform.position, targetPosition);
        element.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder++;

        while (distance > 0.01f)
        {
            Vector3 newPosition = Vector3.Lerp(element.transform.position, targetPosition, nowDifficulty.elementSpeed * Time.deltaTime);
            element.transform.position = newPosition;
            distance = Vector3.Distance(element.transform.position, targetPosition);
            yield return null;
        }

        statusFlg = "Move";
    }

    private IEnumerator BugerMove(GameObject element)
    {
        Vector3 targetPosition = new(nowDifficulty.elementMoveDistance * (Random.Range(0, 2) >= 1 ? 1 : -1),
            element.transform.position.y, 0f);
        float distance = Vector3.Distance(element.transform.position, targetPosition);
        while (true)
        {
            if (statusFlg.Equals("Replace"))
            {
                statusFlg = "Idle";
                yield break;
            }
            if (statusFlg.Equals("Idle") && NetworkManager.inputFlg)
            {
                statusFlg = "Drop";
                yield break;
            }

            if (distance > 0.01f)
            {
                Vector3 newPosition = Vector3.MoveTowards(element.transform.position, targetPosition, nowDifficulty.elementSpeed * Time.deltaTime);
                element.transform.position = newPosition;
            }
            else
            {
                targetPosition.x *= -1;
            }
            distance = Vector3.Distance(element.transform.position, targetPosition);
            yield return null;
        }
    }

    private IEnumerator BugerDrop(GameObject element)
    {
        AudioManager.Instance.PlaySfx(Sfxs.Transition_in);
        element.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        BurgerElement elementFunc = element.GetComponent<BurgerElement>();

        while (true)
        {
            // 바닥에 완전에 착지했거나, 조리대 바깥쪽으로 빠져나간 경우 상태를 종료한다.
            if (element.transform.position.y < -10f)
            {
                Destroy(element);

                if (healthPoint <= 0)
                {
                    AudioManager.Instance.PlaySfx(Sfxs.GameOver);
                    if (UpStairsSideGameManager.IsLoadedSideGame)
                    {
                        UpStairsSideGameManager.Instacne.EndSideGame();
                    }
                    else
                    {
                        CommonGameManager.Instance.FgStart();
                    }
                }
                else
                {
                    statusFlg = "Supply";
                }
                yield break;
            }

            // 재료가 바닥에 닿고 3초 경과한 경우
            if (elementFunc.StopStatus)
            {
                elementCount++;
                nowElement.GetComponent<BurgerElement>().bugerObj = nowCuttingBoard;
                nowElement.GetComponent<BurgerElement>().SetOffset(nowCuttingBoard.transform.position);
                nowElements.Add(nowElement);
                if (elementCount >= nowDifficulty.maxBugerElement)
                {
                    Debug.Log("버거 컴플릿 실행");
                    statusFlg = "BugerComplete";
                    elementCount = 0;
                }
                else
                {
                    statusFlg = "Supply";
                }

                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator BugerEnd()
    {
        statusFlg = "Supply";
        yield break;
    }

    private IEnumerator BugerComplete(GameObject cuttingBoardEle)
    {
        // 빵 윗 부분이 나오도록 한다.
        GameObject nowBugerTop = Instantiate(bugerTop);
        nowBugerTop.SetActive(true);
        nowBugerTop.transform.SetParent(nowCuttingBoard.transform);
        nowBugerTop.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;

        scoreController.AddPointEach(Vector3.zero, ScoreType.Star);

        Vector3 targetPosition = new(-15f, cuttingBoardEle.transform.position.y, 0f);
        float distance = Vector3.Distance(cuttingBoardEle.transform.position, targetPosition);

        yield return new WaitForSeconds(2f);

        while (Vector3.Distance(cuttingBoardEle.transform.position, targetPosition) > 0.01f)
        {
            Vector3 newPosition = Vector3.Lerp(cuttingBoardEle.transform.position, targetPosition,
                nowDifficulty.elementSpeed * Time.deltaTime);
            cuttingBoardEle.transform.position = newPosition;
            yield return null;
        }

        cuttingBoardEle.SetActive(false);
        foreach (GameObject ele in nowElements)
        {
            ele.SetActive(false);
        }
        nowElements = new();

        if (bugerCount >= nowDifficulty.maxBuger)
        {
            //CommonGameManager.Instance.htpEndFlg = false;


            statusFlg = "Idle";
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

        statusFlg = "BugerStart";
    }

    private IEnumerator BugerReplace()
    {
        nowElement.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        foreach (GameObject ele in nowElements)
        {
            ele.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        statusFlg = "Replace";
        GameObject replacedBoard = nowCuttingBoard;
        while (true)
        {
            if (replacedBoard.transform.position.y > -14f)
            {
                replacedBoard.transform.position = Vector3.MoveTowards(replacedBoard.transform.position, new Vector3(0f, -15f, 0f), 5 * Time.deltaTime);
                yield return null;
            }
            else
            {
                elementCount = 0;
                foreach (GameObject ele in nowElements)
                {
                    Destroy(ele);
                }
                nowElements = new();
                Destroy(nowElement);
                Destroy(replacedBoard);
                statusFlg = "BugerStart";
                yield break;
            }
        }
    }
}