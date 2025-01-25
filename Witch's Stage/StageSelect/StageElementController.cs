using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageElementController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI MusicNameText;
    [SerializeField] private TextMeshProUGUI MusicBPMText;
    [SerializeField] private Image backPanel;
    [SerializeField] private Image[] hasLevelImage = new Image[4];

    private Color32 originalColor;
    private bool isSelected = false;

    private StageData stageData;
    public StageData StageData
    {
        get
        {
            return stageData;
        }
        set
        {
            stageData = value;
            MusicNameText.text = value.stageName;
        }
    }

    void Start()
    {
        originalColor = backPanel.color;
    }

    void Update()
    {
        if (isSelected)
        {
            backPanel.color = new Color32(255, 140, 140, 220);
        }
        else
        {
            backPanel.color = originalColor;
        }
    }

    public void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;
    }

    public void SetHasLevel(bool easy, bool normal, bool hard, bool extream)
    {
        hasLevelImage[0].color = easy ? StageSelectConstants.ORIGINAL_LEVEL_COLORS[0] : StageSelectConstants.NO_LEVEL_COLOR;
        hasLevelImage[1].color = normal ? StageSelectConstants.ORIGINAL_LEVEL_COLORS[1] : StageSelectConstants.NO_LEVEL_COLOR;
        hasLevelImage[2].color = hard ? StageSelectConstants.ORIGINAL_LEVEL_COLORS[2] : StageSelectConstants.NO_LEVEL_COLOR;
        hasLevelImage[3].color = extream ? StageSelectConstants.ORIGINAL_LEVEL_COLORS[3] : StageSelectConstants.NO_LEVEL_COLOR;
    }
}
