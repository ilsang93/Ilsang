using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryButtonController : MonoBehaviour
{
    [SerializeField] private Button originalButton;
    [SerializeField] private Button customButton;
    [SerializeField] private GameObject origianlPanel;
    [SerializeField] private GameObject customPanel;
    [SerializeField] private Color selectedColor;

    [HideInInspector] public ButtonType categoryType = ButtonType.None;

    void Start()
    {
        // originalButton.onClick.AddListener(() =>
        // {
        //     if (categoryType == ButtonType.Original) return;
        //     // FindObjectOfType<SelectLineController>().SetDeselectLine();
        //     FindObjectOfType<StageButtonGenerator>().OnClickCategoryButton();
        //     categoryType = ButtonType.Original;
        //     originalButton.interactable = false;
        //     customButton.interactable = true;

        //     ShowCategory();
        // });

        // customButton.onClick.AddListener(() =>
        // {
        //     if (categoryType == ButtonType.Custom) return;
        //     // FindObjectOfType<SelectLineController>().SetDeselectLine();
        //     FindObjectOfType<StageButtonGenerator>().OnClickCategoryButton();
        //     categoryType = ButtonType.Custom;
        //     originalButton.interactable = true;
        //     customButton.interactable = false;

        //     ShowCategory();
        // });

        // originalButton.interactable = false;
        // customButton.interactable = true;
        categoryType = ButtonType.Original;
        // ShowCategory();
    }

    private void ShowCategory()
    {
        HideCategory();

        GameObject targetPanel;
        switch (categoryType)
        {
            case ButtonType.Custom:
                targetPanel = customPanel;
                customButton.image.color = selectedColor;
                customButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                break;
            case ButtonType.Original:
            default:
                targetPanel = origianlPanel;
                originalButton.image.color = selectedColor;
                originalButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                break;
        }

        targetPanel.SetActive(true);
    }

    private void HideCategory()
    {
        GameObject targetPanel;
        switch (categoryType)
        {
            case ButtonType.Custom:
                targetPanel = origianlPanel;
                originalButton.image.color = Color.white;
                originalButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                break;
            case ButtonType.Original:
            default:
                targetPanel = customPanel;
                // customButton.image.color = Color.white;
                // customButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                break;
        }

        targetPanel.SetActive(false);
    }

    public enum ButtonType
    {
        Original,
        Custom,
        None
    }
}
