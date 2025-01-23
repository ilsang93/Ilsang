using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Button cameraResetButton;
    [SerializeField] private CinemachineCamera editorCamera;
    private readonly float defaultSize = 11.8f;
    private readonly float minScale = 2f;
    private readonly float maxScale = 100f;
    private Vector2 dragStartPos, dragEndPos;
    private float nowValue;

    private void Start()
    {
        zoomSlider.value = 0.1f;
        nowValue = zoomSlider.value;
        cameraResetButton.onClick.AddListener(() =>
        {
            zoomSlider.value = 0.1f;
            editorCamera.transform.DOMove(new Vector3(0, 0, -10), 0.3f);
        });
    }

    private void Update()
    {
        if (nowValue != zoomSlider.value)
        {
            nowValue = zoomSlider.value;
            ZoomCamera(zoomSlider.value);
        }

        //마우스 휠 클릭 후 드래그로 카메라 이동
        if (Input.GetMouseButtonDown(2))
        {
            dragStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))
        {
            dragEndPos = Input.mousePosition;
            Vector2 dragVector = (dragStartPos - dragEndPos) / 20;
            editorCamera.transform.DOMove(new Vector3(transform.position.x + dragVector.x, transform.position.y + dragVector.y, -10), 0.3f);
        }
    }

    public void ZoomCamera(float zoomValue)
    {
        // zoomValue (0~1) 기반으로 해상도 스케일 계산
        float scale = Mathf.Lerp(minScale, maxScale, zoomValue);

        // Pixel Perfect Camera의 해상도 설정
        editorCamera.Lens.OrthographicSize = scale;
    }
}
