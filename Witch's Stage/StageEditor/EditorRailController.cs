using DG.Tweening;
using UnityEngine;

public class EditorRailController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isMouseOver = false;
    private bool isSelected = false;
    private int lineIndex;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (isMouseOver)
        {
            if (Input.GetMouseButtonDown(0) && StageEditorManager.Instance.editorStatus == EditorStatus.EditNote)
            {
                StageEditorManager.Instance.SelectRailRoad(lineIndex);
                NoteTimeLineController.Instance.MoveToSelectedLine(lineIndex);
            }
        }
    }

    private void OnMouseEnter()
    {
        print("Enter");
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        print("Exit");
        isMouseOver = false;
    }

    public void SetLineIndex(int index)
    {
        lineIndex = index;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (isSelected)
        {
            spriteRenderer.DOColor(Color.red, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            spriteRenderer.DOKill();
            SetColorReset();
        }
    }

    private void SetColorReset()
    {
        spriteRenderer.DOKill();
        spriteRenderer.color = Color.white;
    }
}
