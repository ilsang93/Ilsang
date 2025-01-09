using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 카메라 컴포넌트가 있는 오브젝트에서 작동한다.
/// 화면 표시 비율을 16 : 9 해상도로 고정한다.
/// 화면 표시 대상 바깥 영역은 검은색으로 레터박스 처리를 진행한다.
/// </summary>
public class CameraResolution : MonoBehaviour
{
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = (float)Screen.width / Screen.height / (16f / 9f); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            Debug.Log("scalewidth : " + scalewidth + " | rect.width : " + rect.width + " | rect.x : " + rect.x);
            rect.width = scalewidth * rect.width;
            rect.x = (1f - scalewidth) / 2f + (rect.x * scalewidth);
        }
        camera.rect = rect;
    }

    void OnPreCull()
    {
        if (name.Equals("Main Camera"))
        {
            GL.Clear(true, true, Color.black);
        }
    }
}