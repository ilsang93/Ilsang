using GemgemAr;

using PixelCrushers.DialogueSystem;

using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonController : MonoBehaviour
{
    private bool isHandTracking = false;
    private Button continueButton;
    private void Start()
    {
        if (FindObjectOfType<NetworkManager>() && FindObjectOfType<NetworkManager>().connectedFlg)
        {
            isHandTracking = true;
        }
        else
        {
            isHandTracking = false;
        }

        continueButton = GetComponent<Button>();
        continueButton.onClick.AddListener(() =>
        {
            Sequencer.Message("@@@컨티뉴 클릭@@@");
        });
    }

    private void OnEnable()
    {
        if (FindObjectOfType<NetworkManager>() && FindObjectOfType<NetworkManager>().connectedFlg)
        {
            isHandTracking = true;
        }
        else
        {
            isHandTracking = false;
        }
    }

    void Update()
    {
        if (isHandTracking)
        {
            if (NetworkManager.inputFlg)
            {
                continueButton.onClick.Invoke();
            }
        }
    }
}