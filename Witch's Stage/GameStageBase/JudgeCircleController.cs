using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class JudgeCircleController : JudgeCircle
{
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        // sort layer
        GetComponent<MeshRenderer>().sortingLayerName = "JudgeTrailer";
        GetComponent<MeshRenderer>().sortingOrder = 5;

        nowRadius = originRadius;

        GenerateDonut();
    }

    void Update()
    {
        if (playing)
        {
            circleTime += Time.deltaTime;
            if (circleTime < showTime)
            {
                nowRadius = Mathf.Lerp(originRadius, targetFinaleRadius, circleTime / showTime);
                GenerateDonut();
            }
            else
            {
                playing = false;
                gameObject?.SetActive(false);
            }
        }

#if UNITY_EDITOR
        if (myTurn)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
#endif
    }

    public override void OnDisable()
    {
        FindObjectOfType<InputManager>().OnDownZ -= OnInputPressed;
        FindObjectOfType<InputManager>().OnDownX -= OnInputPressed;

        if (isFirstAct)
        {
            isFirstAct = false;
            return;
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter가 이 GameObject에 없습니다.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        // 각 버텍스에서 파티클 생성
        foreach (Vector3 vertex in vertices)
        {
            // 로컬 좌표를 월드 좌표로 변환
            Vector3 worldPosition = transform.TransformPoint(vertex);

            // 파티클 프리팹 인스턴스화
            GameObject particle = Instantiate(particlePrefab, worldPosition, Quaternion.identity);

            // 파티클의 크기 조정
            particle.transform.localScale = Vector3.one;
        }
        if (!isJudged) {
            DoJudge(JudgeResult.Miss);  
        }
        ActiveNextCircle();
    }

    public override void AppendInputEvent()
    {
        FindObjectOfType<InputManager>().OnDownZ += OnInputPressed;
        FindObjectOfType<InputManager>().OnDownX += OnInputPressed;
    }

    public void SetCircleTime(float shrinkTime, float graceTime)
    {
        showTime = shrinkTime + graceTime;
        this.shrinkTime = shrinkTime;
        this.graceTime = graceTime;
    }

    private void OnInputPressed()
    {
        if (myTurn)
        {
            //TODO 판정 선과 일정 거리 안에서만 작동하도록 한다.(너무 멀 떄는 작동하지 않도록 한다.)
            //TODO 판정 시간의 간격에 따라 판정을 진행한다.
            if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 2)
            {
                DoJudge(JudgeResult.Perfect);
                StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 3;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Perfect);
            }
            else if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 10)
            {
                DoJudge(JudgeResult.Good);
                StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 2;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Good);
            }
            else if (Mathf.Abs(circleTime - shrinkTime) <= StageConstant.JUDGE_TIME_UNIT * 20)
            {
                DoJudge(JudgeResult.Bad);
                StageManager.Instance.Score += StageManager.Instance.ScoreUnit * 1;
                StageManager.Instance.JudgeDataAdd(JudgeResult.Bad);
            }
            else
            {
                DoJudge(JudgeResult.Miss);
                StageManager.Instance.JudgeDataAdd(JudgeResult.Miss);
            }
            playing = false;
            gameObject.SetActive(false);
        }
    }

    private void GenerateDonut()
    {
        mesh.Clear();

        float innerRadius = nowRadius - thickness;
        int vertexCount = segments * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        float angleIncrement = Mathf.PI * 2f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleIncrement;

            vertices[i * 2] = new Vector3(Mathf.Cos(angle) * nowRadius, Mathf.Sin(angle) * nowRadius, 0);

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

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}