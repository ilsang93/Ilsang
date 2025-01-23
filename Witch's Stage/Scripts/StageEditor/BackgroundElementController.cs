using UnityEngine;
using UnityEngine.UI;

public class BackgroundElementController : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    private Sprite sprite;
    public Sprite Sprite
    {
        get => sprite;
        set
        {
            sprite = value;
        }
    }

    private void Start()
    {
        selectButton.onClick.AddListener(() =>
        {
            BackgroundSelectController.Instance.nowSprite = sprite;
            BackgroundSelectController.Instance.transform.localScale = Vector3.zero;
        });
    }
}
