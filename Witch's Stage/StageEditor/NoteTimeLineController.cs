using DG.Tweening;
using UnityEngine;

public class NoteTimeLineController : MonoBehaviour
{
    public static NoteTimeLineController Instance { get; private set; }
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject timeLine;
    [SerializeField] private GameObject secondLinePrefab; // 1초선 프리팹
    [SerializeField] private GameObject zdoLinePrefab; // 0.1초 선 프리팹

    private float timeLineLength = 0;
    private float timeLineDistanceEach;


    private RectTransform contentRect;
    private float twoSecondWidth;
    private int pageCount;
    private RectTransform lastLine;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        contentRect = content.GetComponent<RectTransform>();
        twoSecondWidth = contentRect.sizeDelta.x;
        timeLineDistanceEach = twoSecondWidth / 20;
    }

    public void Add2SecondLine()
    {
        if (pageCount == 0)
        {
            // 최초 라인을 추가한다.
            RectTransform startLine = Instantiate(secondLinePrefab, new Vector3(), Quaternion.identity, timeLine.transform).GetComponent<RectTransform>();
            startLine.localPosition = new Vector3(0, 0, 0);
            startLine.offsetMax = new Vector2(startLine.offsetMax.x, -50);
            startLine.offsetMin = new Vector2(startLine.offsetMin.x, 0);
            startLine.GetComponent<SecondLineController>().SetSecondText("0");

            lastLine = startLine;
        }
        else
        {
            // 노트 타임 라인 페이지를 추가한다.
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x + twoSecondWidth, contentRect.sizeDelta.y);
        }

        timeLineLength += 2;

        // 초 라인 2개를 추가한다.
        RectTransform firstLine = Instantiate(secondLinePrefab, new Vector3(), Quaternion.identity, timeLine.transform).GetComponent<RectTransform>(); ;
        firstLine.localPosition = new Vector3((twoSecondWidth / 2 - firstLine.rect.width / 2) * (pageCount * 2 + 1), 0, 0);
        firstLine.offsetMax = new Vector2(firstLine.offsetMax.x, -50);
        firstLine.offsetMin = new Vector2(firstLine.offsetMin.x, 0);
        firstLine.GetComponent<SecondLineController>().SetSecondText((pageCount * 2 + 1).ToString());

        RectTransform secondLine = Instantiate(secondLinePrefab, new Vector3(), Quaternion.identity, timeLine.transform).GetComponent<RectTransform>(); ;
        secondLine.localPosition = new Vector3((twoSecondWidth / 2 - firstLine.rect.width / 2) * (pageCount * 2 + 2), 0, 0);
        secondLine.offsetMax = new Vector2(secondLine.offsetMax.x, -50);
        secondLine.offsetMin = new Vector2(secondLine.offsetMin.x, 0);
        secondLine.GetComponent<SecondLineController>().SetSecondText((pageCount * 2 + 2).ToString());

        // 0.1초 라인 9개 추가
        for (int i = 1; i < 10; i++)
        {
            RectTransform zpoLine = Instantiate(zdoLinePrefab, new Vector3(), Quaternion.identity, firstLine).GetComponent<RectTransform>();
            // lastLine과 firstLine 사이에 9개의 0.1초 라인을 추가한다.
            // zpoLine.localPosition = new Vector3((firstLine.localPosition.x - lastLine.localPosition.x) / 10 * i + lastLine.localPosition.x, 0, 0);
            zpoLine.localPosition = new Vector3((firstLine.localPosition.x - lastLine.localPosition.x) / 10 * i - twoSecondWidth / 2, 0, 0);
            zpoLine.offsetMax = new Vector2(zpoLine.offsetMax.x, -100);
            zpoLine.offsetMin = new Vector2(zpoLine.offsetMin.x, 0);
            zpoLine.GetComponent<SecondLineController>().SetSecondText((pageCount * 2).ToString() + "." + i.ToString());
        }

        for (int i = 1; i < 10; i++)
        {
            RectTransform zpoLine = Instantiate(zdoLinePrefab, new Vector3(), Quaternion.identity, secondLine).GetComponent<RectTransform>();
            // firstLine과 secondLine 사이에 9개의 0.1초 라인을 추가한다.
            // zpoLine.localPosition = new Vector3((secondLine.localPosition.x - firstLine.localPosition.x) / 10 * i + firstLine.localPosition.x, 0, 0);
            zpoLine.localPosition = new Vector3((secondLine.localPosition.x - firstLine.localPosition.x) / 10 * i - twoSecondWidth / 2, 0, 0);
            zpoLine.offsetMax = new Vector2(zpoLine.offsetMax.x, -100);
            zpoLine.offsetMin = new Vector2(zpoLine.offsetMin.x, 0);
            zpoLine.GetComponent<SecondLineController>().SetSecondText((pageCount * 2 + 1).ToString() + "." + i.ToString());
        }

        lastLine = secondLine;
        pageCount++;
    }

    public void MoveToSelectedLine(int lineIndex)
    {
        contentRect.DOAnchorPos(new Vector2(-twoSecondWidth * lineIndex, 0), 0.3f);
    }

    public void Remove2SecondLine()
    {
        if (pageCount == 0)
        {
            return;
        }
        if (pageCount != 1)
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x - twoSecondWidth, contentRect.sizeDelta.y);

        // 2초 라인을 제거한다.
        lastLine = timeLine.transform.GetChild(timeLine.transform.childCount - 3).GetComponent<RectTransform>();

        timeLineLength -= 2;
        pageCount--;


        // 마지막의 두 줄 삭제
        Destroy(timeLine.transform.GetChild(timeLine.transform.childCount - 1).gameObject);
        Destroy(timeLine.transform.GetChild(timeLine.transform.childCount - 2).gameObject);
    }

    public void AddNote()
    {
    }
}
