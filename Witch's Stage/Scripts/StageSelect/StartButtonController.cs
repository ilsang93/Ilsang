using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject buttonImage;
    private Material material;
    private bool isMouseOver = false;
    private float scrollValue = 0f;
    // Start is called before the first frame update
    public void Start()
    {
        material = Instantiate(buttonImage.GetComponent<Image>().materialForRendering);
        buttonImage.GetComponent<Image>().material = material;
        material.SetFloat("_ScrollValue", 0f);

        GetComponent<Button>().onClick.AddListener(() =>
        {
            StageSelectManager.Instance.OnClickPlayButton();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (isMouseOver)
        {
            scrollValue += Time.deltaTime;
            material.SetFloat("_ScrollValue", scrollValue);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("Mouse Enter");
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("Mouse Exit");
        isMouseOver = false;
        scrollValue = 0f;
        material.SetFloat("_ScrollValue", 0f);
    }
}
