using System;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    private Material _material;
    private bool _isSelected = false;

    public StageData StageData { get => stageData; }
    private StageData stageData;

    internal Action onClicked;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            onClicked?.Invoke();
        });
    }

    public void InitButton(StageData stageData)
    {
        Image image = GetComponent<Image>();
        image.material = Instantiate(image.materialForRendering);
        _material = image.materialForRendering;

        this.stageData = stageData;
        _material.SetColor("_ButtonColor", StageData.stageButtonColor);
    }

    public void SelectButton()
    {
        if (_isSelected) return;

        FindObjectOfType<SelectLineController>().SetSelectLinePosition(transform.position, StageData.stageButtonColor - new Color(0.3f, 0.3f, 0.3f, 0.0f) + new Color(0f, 0f, 0f, 0.6f));
        _material.SetInt("_Selected", 1);
        _isSelected = true;
    }

    public void DeselectButton()
    {
        _material.SetInt("_Selected", 0);
        _isSelected = false;
    }
}
