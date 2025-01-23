using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private float scrollSpeed = 0.1f;

    void Start()
    {
        slider = GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            slider.value += Input.mouseScrollDelta.y * -1 * scrollSpeed;
        }
    }
}
