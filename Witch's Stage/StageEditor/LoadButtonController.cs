using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadButtonController : MonoBehaviour
{
    public Action OnLoadButtonClicked;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnLoadButtonClicked?.Invoke();
        });
    }
}
