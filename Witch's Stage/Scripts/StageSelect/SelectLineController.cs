using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class SelectLineController : MonoBehaviour
{
    [SerializeField] private GameObject selectLineVertical;
    [SerializeField] private GameObject selectLineHorizontal;
    [SerializeField] private GameObject cursor;

    private Vector2 verticalDefaultPos;
    private Vector2 horizontalDefaultPos;
    private Quaternion verticalDefaultRot;
    private Quaternion horizontalDefaultRot;
    private Vector3 cursorDefaultPos;

    private void Start()
    {
        cursor.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        selectLineVertical.GetComponent<Image>().material = new Material(selectLineVertical.GetComponent<Image>().materialForRendering);
        selectLineHorizontal.GetComponent<Image>().material = new Material(selectLineHorizontal.GetComponent<Image>().materialForRendering);
        SetDefaultPosAndRot();
    }

    private void SetDefaultPosAndRot()
    {
        verticalDefaultPos = selectLineVertical.transform.position;
        verticalDefaultRot = selectLineVertical.transform.rotation;
        horizontalDefaultPos = selectLineHorizontal.transform.position;
        horizontalDefaultRot = selectLineHorizontal.transform.rotation;

        cursorDefaultPos = cursor.transform.position;
    }

    public void SetSelectLinePosition(Vector3 position, Color color)
    {

        selectLineVertical.GetComponent<Image>().materialForRendering.SetColor("_MainColor", color + new Color(0.1f, 0.1f, 0.1f, 0.0f));
        selectLineHorizontal.GetComponent<Image>().materialForRendering.SetColor("_MainColor", color - new Color(0.1f, 0.1f, 0.1f, 0.0f));
        selectLineVertical.GetComponent<Image>().materialForRendering.SetColor("_TextColor", new Color(1, 1, 1, 1) - (color - new Color(0.1f, 0.1f, 0.1f, 0.0f)) + new Color(0, 0, 0, 1f));
        selectLineHorizontal.GetComponent<Image>().materialForRendering.SetColor("_TextColor", new Color(1, 1, 1, 1) - (color - new Color(0.1f, 0.1f, 0.1f, 0.0f)) + new Color(0, 0, 0, 1f));

        selectLineVertical.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 80f)), 0.1f);
        selectLineHorizontal.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(275f, 355f)), 0.1f);

        cursor.transform.DOMove(position, 0.2f).SetEase(Ease.OutElastic);

        selectLineVertical.transform.DOMove(position, 0.5f).SetEase(Ease.OutElastic);
        selectLineHorizontal.transform.DOMove(position, 0.4f).SetEase(Ease.OutElastic);
    }

    /// <summary>
    /// 커서와 선택 연출 선의 위치와 회전값을 초기화한다.
    /// </summary>
    public void SetOriginPosition()
    {
        cursor.transform.position = cursorDefaultPos;
        selectLineVertical.transform.SetPositionAndRotation(verticalDefaultPos, verticalDefaultRot);
        selectLineHorizontal.transform.SetPositionAndRotation(horizontalDefaultPos, horizontalDefaultRot);
    }
}
