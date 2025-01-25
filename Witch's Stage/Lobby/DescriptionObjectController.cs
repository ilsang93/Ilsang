using DG.Tweening;
using TMPro;
using UnityEngine;

public class DescriptionObjectController : MonoBehaviour
{
    public SpriteRenderer leftSprite;
    public SpriteRenderer centerSprite;
    public SpriteRenderer rightSprite;
    public TextMeshPro textMesh;
    [SerializeField] private bool isAutoOpen = false;


    void Start()
    {
        // 사전 정의 텍스트에 대한 대화 상자 재정렬
        if (!string.IsNullOrEmpty(textMesh.text))
        {
            UpdateDialogueBox();
        }

        transform.localScale = Vector3.zero;
    }

    void UpdateDialogueBox()
    {
        float leftSpriteWidth = leftSprite.sprite.bounds.size.x;
        float rightSpriteWidth = rightSprite.sprite.bounds.size.x;
        float centerSpriteUnitWidth = centerSprite.sprite.bounds.size.x;

        // 텍스트의 가로 길이를 계산
        float textWidth = textMesh.preferredWidth;

        // 가운데 스프라이트의 너비를 텍스트에 맞춰 조정
        int repeatCount = Mathf.CeilToInt(textWidth / centerSpriteUnitWidth);
        float centerSpriteWidth = repeatCount * centerSpriteUnitWidth;

        // 대화 상자 전체의 너비 계산
        float totalWidth = leftSpriteWidth + centerSpriteWidth + rightSpriteWidth;

        // 왼쪽 스프라이트 위치 설정
        leftSprite.transform.localPosition = new Vector3(-totalWidth / 2 + leftSpriteWidth / 2, 0, 0);

        // 가운데 스프라이트 위치 및 스케일 설정
        centerSprite.transform.localPosition = new Vector3(0, 0, 0);
        centerSprite.transform.localScale = new Vector3(centerSpriteWidth / centerSpriteUnitWidth, 1, 1);

        // 오른쪽 스프라이트 위치 설정
        rightSprite.transform.localPosition = new Vector3(totalWidth / 2 - rightSpriteWidth / 2, 0, 0);

        // 텍스트의 위치도 가운데에 맞춰서 조정
        textMesh.rectTransform.sizeDelta = new Vector2(textWidth, textMesh.rectTransform.sizeDelta.y);
        textMesh.transform.localPosition = centerSprite.transform.localPosition;
    }

    // 외부에서 이 메서드를 호출하여 대화 상자를 업데이트할 수 있음
    public void SetDialogueText(string dialogue)
    {
        textMesh.text = dialogue;
        UpdateDialogueBox();
    }

    public void OpenWindow()
    {
        if (transform.localScale == Vector3.one) return;
        transform.DOScale(Vector3.one / 5, 0.3f);
    }

    public void CloseWindow()
    {
        if (transform.localScale == Vector3.zero) return;
        transform.DOScale(Vector3.zero, 0.3f);
    }
}

#if UNITY_EDITOR
public static class DescriptionObjectControllerEditor
{
    [UnityEditor.MenuItem("resizing/DescriptionObject")]
    static void UpdateDialogueBox()
    {
        DescriptionObjectController descriptionObjectController = GameObject.FindObjectOfType<DescriptionObjectController>();

        if (string.IsNullOrEmpty(descriptionObjectController.textMesh.text)) return;

        float leftSpriteWidth = descriptionObjectController.leftSprite.sprite.bounds.size.x;
        float rightSpriteWidth = descriptionObjectController.rightSprite.sprite.bounds.size.x;
        float centerSpriteUnitWidth = descriptionObjectController.centerSprite.sprite.bounds.size.x;

        // 텍스트의 가로 길이를 계산
        float textWidth = descriptionObjectController.textMesh.preferredWidth;

        // 가운데 스프라이트의 너비를 텍스트에 맞춰 조정
        int repeatCount = Mathf.CeilToInt(textWidth / centerSpriteUnitWidth);
        float centerSpriteWidth = repeatCount * centerSpriteUnitWidth;

        // 대화 상자 전체의 너비 계산
        float totalWidth = leftSpriteWidth + centerSpriteWidth + rightSpriteWidth;

        // 왼쪽 스프라이트 위치 설정
        descriptionObjectController.leftSprite.transform.localPosition = new Vector3(-totalWidth / 2 + leftSpriteWidth / 2, 0, 0);

        // 가운데 스프라이트 위치 및 스케일 설정
        descriptionObjectController.centerSprite.transform.localPosition = new Vector3(0, 0, 0);
        descriptionObjectController.centerSprite.transform.localScale = new Vector3(centerSpriteWidth / centerSpriteUnitWidth, 1, 1);

        // 오른쪽 스프라이트 위치 설정
        descriptionObjectController.rightSprite.transform.localPosition = new Vector3(totalWidth / 2 - rightSpriteWidth / 2, 0, 0);

        // 텍스트의 위치도 가운데에 맞춰서 조정
        descriptionObjectController.textMesh.rectTransform.sizeDelta = new Vector2(textWidth, descriptionObjectController.textMesh.rectTransform.sizeDelta.y);
        descriptionObjectController.textMesh.transform.localPosition = descriptionObjectController.centerSprite.transform.localPosition;
    }
}
#endif