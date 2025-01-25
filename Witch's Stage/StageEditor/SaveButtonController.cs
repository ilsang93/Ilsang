using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonController : MonoBehaviour
{
    public Action OnSaveButtonClicked;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnSaveButtonClicked?.Invoke();
        });
    }
}
