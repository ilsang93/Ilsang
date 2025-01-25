using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageLoadElementController : MonoBehaviour
{
    [SerializeField] private Button loadButton;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private TextMeshProUGUI updateTimeText;
    [SerializeField] private TextMeshProUGUI authorText;

    private StageData stageData;
    // Start is called before the first frame update
    void Start()
    {
        loadButton.onClick.AddListener(() =>
        {
            LoadStage(stageData);
        });
    }

    private void LoadStage(StageData stageData)
    {
        DataLoadDialogManager.Instance.LoadData(stageData);
    }

    public void SetStageData(StageData stageData)
    {
        this.stageData = stageData;
        stageNameText.text = stageData.stageName;
        updateTimeText.text = stageData.updateTime;
        authorText.text = stageData.author;
    }
}
