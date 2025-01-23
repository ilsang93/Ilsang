using UnityEngine;
using UnityEngine.UI;

public class KeyPressedUIController : MonoBehaviour
{
    [SerializeField] private GameObject mouse;
    [SerializeField] private GameObject keyboard_Z;
    [SerializeField] private GameObject keyboard_X;

    [SerializeField] private Sprite keyUpSprite;
    [SerializeField] private Sprite keyDownSprite;
    [SerializeField] private Sprite mouseIdleSprite;
    [SerializeField] private Sprite mouseLeftSprite;
    [SerializeField] private Sprite mouseRightSprite;
    [SerializeField] private Sprite mouseBothSprite;

    private readonly float keyUpTextPosY = 10f;
    private readonly float keyDownTextPosY = -15f;
    private RectTransform xTextRectTransform, zTextRectTransform;

    private bool leftClick, rightClick;
    private Image mouseImage;

    private void Start()
    {
        InputManager inputManager = FindObjectOfType<InputManager>();
        mouseImage = mouse.GetComponent<Image>();
        zTextRectTransform = keyboard_Z.transform.GetChild(0).GetComponent<RectTransform>();
        xTextRectTransform = keyboard_X.transform.GetChild(0).GetComponent<RectTransform>();
        
        inputManager.OnDownZ += () =>
        {
            keyboard_Z.GetComponent<Image>().sprite = keyDownSprite;
            zTextRectTransform.anchoredPosition = new Vector3(0, keyDownTextPosY, 0);
        };
        inputManager.OnUpZ += () =>
        {
            keyboard_Z.GetComponent<Image>().sprite = keyUpSprite;
            zTextRectTransform.anchoredPosition = new Vector3(0, keyUpTextPosY, 0);
        };
        inputManager.OnDownX += () =>
        {
            keyboard_X.GetComponent<Image>().sprite = keyDownSprite;
            xTextRectTransform.anchoredPosition = new Vector3(0, keyDownTextPosY, 0);
        };
        inputManager.OnUpX += () =>
        {
            keyboard_X.GetComponent<Image>().sprite = keyUpSprite;
            xTextRectTransform.anchoredPosition = new Vector3(0, keyUpTextPosY, 0);
        };
        
        inputManager.OnDownMouseLeft += () =>
        {
            leftClick = true;
            DoMouseChangeStatus();
        };
        inputManager.OnUpMouseLeft += () =>
        {
            leftClick = false;
            DoMouseChangeStatus();
        };
        inputManager.OnDownMouseRight += () =>
        {
            rightClick = true;
            DoMouseChangeStatus();
        };
        inputManager.OnUpMouseRight += () =>
        {
            rightClick = false;
            DoMouseChangeStatus();
        };
    }

    private void DoMouseChangeStatus()
    {
        if (leftClick && rightClick)
        {
            mouseImage.sprite = mouseBothSprite;
        }
        else if (leftClick)
        {
            mouseImage.sprite = mouseLeftSprite;
        }
        else if (rightClick)
        {
            mouseImage.sprite = mouseRightSprite;
        }
        else
        {
            mouseImage.sprite = mouseIdleSprite;
        }
    }
}
