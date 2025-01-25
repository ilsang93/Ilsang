using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SecondLineController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI secondText;
    public float Time
    {
        get
        {
            return secondText.text == "" ? 0 : float.Parse(secondText.text);
        }
    }
    public bool isSelected = false;
    private GameObject thisTlNote;
    public GameObject ThisTlNote
    {
        get
        {
            return thisTlNote;
        }

        set
        {
            thisTlNote = value;
            thisTlNote.GetComponent<EditorNoteController>().isTimeLineNote = true;
            thisTlNote.GetComponent<EditorNoteController>().onClickAction += SetSelectedByNote;
        }
    }

    private GameObject thisFlNote;
    public GameObject ThisFlNote
    {
        get
        {
            return thisFlNote;
        }

        set
        {
            thisFlNote = value;
            thisFlNote.GetComponent<EditorNoteController>().isTimeLineNote = false;
            thisFlNote.GetComponent<EditorNoteController>().onClickAction += SetSelectedByNote;
        }
    }

    private Image image;
    private Color originColor;

    private void Start()
    {
        image = GetComponent<Image>();
        originColor = image.color;
    }

    public void SetSecondText(string text)
    {
        secondText.text = text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StageEditorManager.Instance.editorStatus == EditorStatus.EditNote && !isSelected)
        {
            StageEditorManager.Instance.SelectSecondLine(this);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (isSelected)
        {
            image.DOColor(Color.blue, 0.5f).SetLoops(-1, LoopType.Yoyo);
            if (thisTlNote && thisFlNote)
            {
                thisTlNote.GetComponent<EditorNoteController>().SetSelected(true);
                thisFlNote.GetComponent<EditorNoteController>().SetSelected(true);
            }
        }
        else
        {
            SetColorReset();
        }
    }

    public void SetSelectedByNote()
    {
        StageEditorManager.Instance.DeselectAllRailRoad();
        StageEditorManager.Instance.SelectSecondLine(this);
        EditorNoteController[] editorNoteController = FindObjectsOfType<EditorNoteController>();
        foreach (EditorNoteController noteController in editorNoteController)
        {
            noteController.SetSelected(false);
        }
        thisTlNote.GetComponent<EditorNoteController>().SetSelected(true);
        thisFlNote.GetComponent<EditorNoteController>().SetSelected(true);
    }

    public void SetColorReset()
    {
        image.DOKill();
        image.color = originColor;
    }
}
