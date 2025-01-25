using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuSliderController : MonoBehaviour
{
    [SerializeField] private Button sliderButton;
    [SerializeField] private Image sliderArrow;
    private bool isSliderOpen = true;

    private const float SLIDER_OPEN_X = 0;
    private const float SLIDER_CLOSE_X = -500;
    // Start is called before the first frame update
    void Start()
    {
        sliderButton.onClick.AddListener(() =>
        {
            if (isSliderOpen)
            {
                sliderArrow.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
                GetComponent<RectTransform>().DOAnchorPosX(SLIDER_CLOSE_X, 0.5f);
                isSliderOpen = false;
            }
            else
            {
                sliderArrow.rectTransform.rotation = Quaternion.Euler(0, 180, 0);
                GetComponent<RectTransform>().DOAnchorPosX(SLIDER_OPEN_X, 0.5f);
                isSliderOpen = true;
            }
        });
    }
}
