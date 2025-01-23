using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelRIghtController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private Image stageImage;
    [SerializeField] private LevelIconController levelIconController;

    /// <summary>
    /// 우측 패널에 스테이지 데이터 정보를 세팅한다.
    /// </summary>
    /// <param name="stageData"></param>
    public void SetStageData(Difficulty difficulty = Difficulty.Easy)
    {
        ResetStageDataPanel();
        stageNameText.text = StageSelectManager.Instance.selectedStageData.stageName;
        //TODO 음악 자켓 이미지를 세팅한다.
        //TODO 난이도 아이콘을 세팅한다.
        FindObjectOfType<DifficultySliderController>().SetDifficulty((Difficulty)difficulty);
    }

    /// <summary>
    /// 우측 패널의 스테이지 데이터를 초기화한다.
    /// </summary>
    private void ResetStageDataPanel()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public void ChangeStageDifficulty(int difficulty)
    {
        levelIconController.SetLevel(StageSelectManager.Instance.selectedStageData.levelData[difficulty].level);
    }
}
