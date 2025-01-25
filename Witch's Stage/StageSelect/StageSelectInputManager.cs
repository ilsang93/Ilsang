using System;
using UnityEngine;

public class StageSelectInputManager : MonoBehaviour
{
    public Action OnLeftButtonPressed;
    public Action OnRightButtonPressed;
    public Action OnUpButtonPressed;
    public Action OnUpButtonHold;
    public Action OnDownButtonPressed;
    public Action OnDownButtonHold;
    public Action OnReleaseUpDownButton;
    public Action OnSubmitButtonPressed;
    public Action OnSpeedButtonPressed;

    private bool releasedFlg = true;

    //FIXME 언젠가 키 설정을 변경할 수 있도록 해야 함.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnLeftButtonPressed?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnRightButtonPressed?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            OnUpButtonPressed?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            OnDownButtonPressed?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmitButtonPressed?.Invoke();
        }

        if (Input.GetKey(KeyCode.W))
        {
            print("HoldUpButton");
            OnUpButtonHold?.Invoke();
            releasedFlg = false;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            OnDownButtonHold?.Invoke();
            releasedFlg = false;
        }
        else
        {
            if (!releasedFlg)
            {
                OnReleaseUpDownButton?.Invoke();
                releasedFlg = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            OnSpeedButtonPressed?.Invoke();
        }
    }
}
