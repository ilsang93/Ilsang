using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action OnDownZ;
    public Action OnUpZ;
    public Action OnDownX;
    public Action OnUpX;
    public Action OnHoldZ;
    public Action OnHoldX;
    public Action OnDownMouseLeft;
    public Action OnUpMouseLeft;
    public Action OnDownMouseRight;
    public Action OnUpMouseRight;
    public Action OnHoldMouseLeft;
    public Action OnHoldMouseRight;
    public Action OnHoldMouseBoth;
    public Action OnReleaseMouseBoth;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnDownZ?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            OnUpZ?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnDownX?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            OnUpX?.Invoke();
        }
        
        if (Input.GetKey(KeyCode.Z))
        {
            OnHoldZ?.Invoke();
        }
        if (Input.GetKey(KeyCode.X))
        {
            OnHoldX?.Invoke();
        }

        // 마우스 입력 이벤트
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            OnHoldMouseBoth?.Invoke();
        }
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            OnReleaseMouseBoth?.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            OnHoldMouseLeft?.Invoke();
        }
        if (Input.GetMouseButton(1))
        {
            OnHoldMouseRight?.Invoke();
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnDownMouseLeft?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnUpMouseLeft?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnDownMouseRight?.Invoke();
        }
        if (Input.GetMouseButtonUp(1))
        {
            OnUpMouseRight?.Invoke();
        }
    }
}
