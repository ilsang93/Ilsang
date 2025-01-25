using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class JudgeLongCircleController : JudgeCircle
{
    [SerializeField] private GameObject circleStart;
    [SerializeField] private GameObject circleEnd;
    [SerializeField] private GameObject circleBetween;

    [SerializeField] private Material materialEdge;
    [SerializeField] private Material materialBetween;
    [SerializeField] private Material materialEdgeFailed;
    [SerializeField] private Material materialBetweenFailed;
    [SerializeField] private Color failedColor;

    public float targetFinalRadius = 3f;

    private float nowStartRadius = 8f;
    private float nowEndRadius = 8f;
    private float holdRadius = 0f;

    private float endTime;

    private Mesh meshStart;
    private Mesh meshEnd;
    private Mesh meshBetween;

    private bool isHold = false;
    private bool isLeft = false; // true: left, false: right
    private Vector3[] betweenVertices;
    private bool[] handlerFlg = new bool[6] {
        false, // OnDownZ 
        false, // OnDownX  
        false, // OnHoldZ 
        false, // OnHoldX 
        false, // OnUpZ
        false  // OnUpX
    };
    private JudgeResult pressResult;

    private void Start()
    {
        meshStart = new Mesh();
        meshEnd = new Mesh();
        meshBetween = new Mesh();

        circleStart.GetComponent<MeshFilter>().mesh = meshStart;
        circleEnd.GetComponent<MeshFilter>().mesh = meshEnd;
        circleBetween.GetComponent<MeshFilter>().mesh = meshBetween;

        circleStart.GetComponent<MeshRenderer>().material = materialEdge;
        circleEnd.GetComponent<MeshRenderer>().material = materialEdge;
        circleBetween.GetComponent<MeshRenderer>().material = materialBetween;

        circleStart.GetComponent<MeshRenderer>().sortingLayerName = "JudgeTrailer";
        circleStart.GetComponent<MeshRenderer>().sortingOrder = 5;

        circleEnd.GetComponent<MeshRenderer>().sortingLayerName = "JudgeTrailer";
        circleEnd.GetComponent<MeshRenderer>().sortingOrder = 5;

        circleBetween.GetComponent<MeshRenderer>().sortingLayerName = "JudgeTrailer";
        circleBetween.GetComponent<MeshRenderer>().sortingOrder = 4;

        betweenVertices = new Vector3[segments * 2];

        endTime = shrinkTime + longTime;

        nowStartRadius = originRadius;
        nowEndRadius = originRadius;

        GenerateMeshStart();
        GenerateMeshEnd();
        GenerateMeshBetween();
    }

    void Update()
    {
        if (playing)
        {
            circleTime += Time.deltaTime;

            if (circleTime < endTime)
            {
                // 홀드 중일 때, 출발원은 홀드 시점에 고정한다.
                if (isHold)
                {
                    nowStartRadius = holdRadius;
                }
                else
                {
                    nowStartRadius = circleTime > shrinkTime ? targetRadius : Mathf.Lerp(originRadius, targetFinalRadius, circleTime / shrinkTime);
                }

                nowEndRadius = circleTime < endTime - shrinkTime ? originRadius : Mathf.Lerp(targetRadius, originRadius, (endTime - circleTime) / shrinkTime);

                GenerateLongCircleSet();
            }
            else
            {
                switch (pressResult)
                {
                    case JudgeResult.Perfect:
                        StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 3;
                        break;
                    case JudgeResult.Good:
                        StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 2;
                        break;
                    case JudgeResult.Bad:
                        StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 1;
                        break;
                    case JudgeResult.Miss:
                        StageManager.Instance.JudgeDataAdd(JudgeResult.Miss);
                        break;
                    default:
                        break;
                }

                playing = false;
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 입력 처리
    /// 세팅 기능
    /// 완료 후 기능
    /// </summary>
    public override void OnDisable()
    {
        if (handlerFlg[0]) FindObjectOfType<InputManager>().OnDownZ -= () => OnInputPressed(true);
        if (handlerFlg[1]) FindObjectOfType<InputManager>().OnDownX -= () => OnInputPressed(false);
        if (handlerFlg[2]) FindObjectOfType<InputManager>().OnHoldZ -= () => OnInputHold(true);
        if (handlerFlg[3]) FindObjectOfType<InputManager>().OnHoldX -= () => OnInputHold(false);
        if (handlerFlg[4]) FindObjectOfType<InputManager>().OnUpZ -= () => OnInputReleased(true);
        if (handlerFlg[5]) FindObjectOfType<InputManager>().OnUpX -= () => OnInputReleased(false);

        if (isFirstAct)
        {
            isFirstAct = false;
            return;
        }
    }

    public override void AppendInputEvent()
    {
        FindObjectOfType<InputManager>().OnDownZ += () => OnInputPressed(true);
        FindObjectOfType<InputManager>().OnDownX += () => OnInputPressed(false);
        FindObjectOfType<InputManager>().OnHoldZ += () => OnInputHold(true);
        FindObjectOfType<InputManager>().OnHoldX += () => OnInputHold(false);
        FindObjectOfType<InputManager>().OnUpZ += () => OnInputReleased(true);
        FindObjectOfType<InputManager>().OnUpX += () => OnInputReleased(false);

        Array.Fill(handlerFlg, true);
    }

    public override void DoJudge(JudgeResult result)
    {
        switch (result)
        {
            case JudgeResult.Perfect:
                Debug.Log("Perfect");
                GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Perfect";
                if (!isHold) StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 3;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Perfect);
                break;
            case JudgeResult.Good:
                Debug.Log("Good");
                GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Good";
                if (!isHold) StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 2;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Good);
                break;
            case JudgeResult.Bad:
                Debug.Log("Bad");
                GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Bad";
                if (!isHold) StageManager.Instance.Score += StageManager.Instance.ScoreUnit;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Bad);
                break;
            case JudgeResult.Miss:
                Debug.Log("Miss");
                GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Miss";
                StageManager.Instance.JudgeDataAdd(JudgeResult.Miss);
                break;
        }
        isJudged = true;
    }

    private void OnInputPressed(bool isLeft)
    {
        if (myTurn)
        {
            // 불필요한 입력 이벤트를 제거한다.
            FindObjectOfType<InputManager>().OnDownZ -= () => OnInputPressed(true);
            FindObjectOfType<InputManager>().OnDownX -= () => OnInputPressed(false);

            handlerFlg[0] = false;
            handlerFlg[1] = false;

            if (isLeft)
            {
                FindObjectOfType<InputManager>().OnHoldX -= () => OnInputHold(false);
                FindObjectOfType<InputManager>().OnUpX -= () => OnInputReleased(false);

                handlerFlg[3] = false;
                handlerFlg[5] = false;
            }
            else
            {
                FindObjectOfType<InputManager>().OnHoldZ -= () => OnInputHold(true);
                FindObjectOfType<InputManager>().OnUpZ -= () => OnInputReleased(true);

                handlerFlg[2] = false;
                handlerFlg[4] = false;
            }

            // 입력한 키를 보존한다.
            this.isLeft = isLeft;

            pressResult = JudgeResult.Miss;
            // 판정이 맞는지 확인한다
            if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 2)
            {
                DoJudge(pressResult = JudgeResult.Perfect);
            }
            else if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 10)
            {
                DoJudge(pressResult = JudgeResult.Good);
            }
            else if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 20)
            {
                DoJudge(pressResult = JudgeResult.Bad);
            }

            // 맞는 경우
            if (pressResult != JudgeResult.Miss)
            {
                isHold = true;
                holdRadius = nowStartRadius;
            }
            else
            {
                ComboManager.instance.Miss();
                DoJudge(pressResult = JudgeResult.Miss);
                // 아닌 경우
                // 회색 투명하게 색을 변경한다.
                circleStart.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleEnd.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleBetween.GetComponent<MeshRenderer>().material = materialBetweenFailed;
                // 실패 연출을 한다.
            }

            ActiveNextCircle();
        }
    }

    private float holdTime = 0;
    private float effectInterval = 0.2f;
    private void OnInputHold(bool isLeft)
    {
        if (isHold && this.isLeft == isLeft)
        {
            holdTime += Time.deltaTime;
            if (holdTime > effectInterval)
            {
                holdTime = 0;
                ComboManager.instance.AddCombo();
                StageManager.Instance.JudgeDataAdd(pressResult);
                // 이펙트를 생성한다.
                GameObject particle = Instantiate(particlePrefab, transform);
                particle.transform.localPosition = Vector2.zero;
                particle.transform.localScale = Vector3.one * 3f;
                particle.GetComponent<ParticleSystemRenderer>().sortingLayerName = "JudgeTrailer";
                Destroy(particle, 1f);
            }
        }
    }

    private void OnInputReleased(bool isLeft)
    {
        if (isHold && this.isLeft == isLeft)
        {
            isHold = false;

            FindObjectOfType<InputManager>().OnHoldZ -= () => OnInputHold(true);
            FindObjectOfType<InputManager>().OnHoldX -= () => OnInputHold(false);
            FindObjectOfType<InputManager>().OnUpZ -= () => OnInputReleased(true);
            FindObjectOfType<InputManager>().OnUpX -= () => OnInputReleased(false);

            handlerFlg[2] = false;
            handlerFlg[3] = false;
            handlerFlg[4] = false;
            handlerFlg[5] = false;

            if (circleTime < endTime - shrinkTime)
            {
                DoJudge(pressResult = JudgeResult.Miss);
                ComboManager.instance.Miss();
                circleStart.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleEnd.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleBetween.GetComponent<MeshRenderer>().material = materialBetweenFailed;
            }
        }
    }

    private void GenerateLongCircleSet()
    {
        bool startCircleStoped = circleTime > shrinkTime;
        bool endCircleStoped = circleTime < endTime - shrinkTime;

        // Generate start circle
        if (!startCircleStoped)
        {
            if (!isHold)
            {
                GenerateMeshStart();
            }
        }
        else
        {
            if (!isJudged)
            {
                circleStart.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleEnd.GetComponent<MeshRenderer>().material = materialEdgeFailed;
                circleBetween.GetComponent<MeshRenderer>().material = materialBetweenFailed;
                DoJudge(pressResult = JudgeResult.Miss);
                ComboManager.instance.Miss();
                ActiveNextCircle();
            }
        }

        // 바깥쪽 원은 
        if (!endCircleStoped)
        {
            GenerateMeshEnd();
        }

        // Generate between circle
        GenerateMeshBetween();
    }

    private void GenerateMeshStart()
    {
        meshStart.Clear();

        float innerRadius = nowStartRadius - thickness;
        int vertexCount = segments * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleIncrement = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleIncrement;

            vertices[i * 2] = new Vector3(Mathf.Cos(angle) * nowStartRadius, Mathf.Sin(angle) * nowStartRadius, 0);
            betweenVertices[i * 2 + 1] = vertices[i * 2];

            vertices[i * 2 + 1] = new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);

            int nextIndex = (i + 1) % segments;
            int triangleIndex = i * 6;

            triangles[triangleIndex] = i * 2;
            triangles[triangleIndex + 1] = nextIndex * 2;
            triangles[triangleIndex + 2] = i * 2 + 1;

            triangles[triangleIndex + 3] = nextIndex * 2;
            triangles[triangleIndex + 4] = nextIndex * 2 + 1;
            triangles[triangleIndex + 5] = i * 2 + 1;
        }

        meshStart.vertices = vertices;
        meshStart.triangles = triangles;
        meshStart.RecalculateNormals();
    }

    private void GenerateMeshEnd()
    {
        meshEnd.Clear();

        float innerRadius = nowEndRadius - thickness;
        int vertexCount = segments * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleIncrement = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleIncrement;

            vertices[i * 2] = new Vector3(Mathf.Cos(angle) * nowEndRadius, Mathf.Sin(angle) * nowEndRadius, 0);

            vertices[i * 2 + 1] = new Vector3(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius, 0);
            betweenVertices[i * 2] = vertices[i * 2 + 1];

            int nextIndex = (i + 1) % segments;
            int triangleIndex = i * 6;

            triangles[triangleIndex] = i * 2;
            triangles[triangleIndex + 1] = nextIndex * 2;
            triangles[triangleIndex + 2] = i * 2 + 1;

            triangles[triangleIndex + 3] = nextIndex * 2;
            triangles[triangleIndex + 4] = nextIndex * 2 + 1;
            triangles[triangleIndex + 5] = i * 2 + 1;
        }

        meshEnd.vertices = vertices;
        meshEnd.triangles = triangles;
        meshEnd.RecalculateNormals();
    }

    private void GenerateMeshBetween()
    {
        meshBetween.Clear();

        int vertexCount = segments * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleIncrement = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            int triangleIndex = i * 6;

            triangles[triangleIndex] = i * 2;
            triangles[triangleIndex + 1] = nextIndex * 2;
            triangles[triangleIndex + 2] = i * 2 + 1;

            triangles[triangleIndex + 3] = nextIndex * 2;
            triangles[triangleIndex + 4] = nextIndex * 2 + 1;
            triangles[triangleIndex + 5] = i * 2 + 1;
        }

        meshBetween.vertices = betweenVertices;
        meshBetween.triangles = triangles;
        meshBetween.RecalculateNormals();
    }
}