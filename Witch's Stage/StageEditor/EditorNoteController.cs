using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorNoteController : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public GameObject pairNote;
    [HideInInspector] public bool isTimeLineNote = false;
    [HideInInspector] public float targetTime;
    private bool isMouseOver = false;

    public Action onClickAction;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // 노트 수정 모드일 때, 마우스 클릭 시.
        if (Input.GetMouseButtonDown(0) && StageEditorManager.Instance.editorStatus == EditorStatus.EditNote && isMouseOver && !isTimeLineNote)
        {
            onClickAction?.Invoke();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (StageEditorManager.Instance.editorStatus == EditorStatus.EditNote && isTimeLineNote)
        {
            onClickAction?.Invoke();
        }
    }

    public void SetPair(GameObject pairNote, float targetTime)
    {
        this.pairNote = pairNote;
        this.targetTime = targetTime;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            SetColor();
        }
        else
        {
            SetColorReset();
        }
    }

    public void SetColor()
    {
        if (isTimeLineNote)
        {
            GetComponent<Image>().DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            GetComponent<SpriteRenderer>().DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void SetColorReset()
    {
        if (isTimeLineNote)
        {
            GetComponent<Image>().DOKill();
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().DOKill();
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    private void OnMouseEnter()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }
}
