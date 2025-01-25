using System.Collections.Generic;
using UnityEngine;

public class StageButtonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject stageButtonPrefab;

    private readonly List<StageButtonController> stageButtonControllers = new();

    public int SelectedStageIndex { get => selectedStageIndex; set => selectedStageIndex = value; }
    private int selectedStageIndex;

    private void Start()
    {
        ParsingOriginalStage();
        ParsingCustomStage();
    }

    /// <summary>
    /// 인 게임 첨부 스테이지를 파싱한다.
    /// </summary>
    private void ParsingOriginalStage()
    {
        StageData[] stageDatas = Resources.LoadAll<StageData>("Datas/StageDatas");

        for (int i = 0; i < stageDatas.Length; i++)
        {

            StageButtonController btnController = Instantiate(stageButtonPrefab).GetComponent<StageButtonController>();
            btnController.onClicked += () => OnClickStageButton(btnController);
            stageButtonControllers.Add(btnController);

            btnController.InitButton(stageDatas[i]);
        }
        SelectedStageIndex = 0;
    }

    /// <summary>
    /// 커스텀 스테이지를 로드한다.
    /// 커스텀 스테이지 기능 확립 후 구현 예정
    /// </summary>
    private void ParsingCustomStage()
    {

    }

    private void OnClickStageButton(StageButtonController stageButtonController)
    {
        foreach (StageButtonController controller in stageButtonControllers)
        {
            controller.DeselectButton();
        }

        stageButtonController.SelectButton();
        SelectedStageIndex = stageButtonControllers.IndexOf(stageButtonController);

        StageSelectManager.Instance.selectedStageData = stageButtonController.StageData;

        FindObjectOfType<PanelRIghtController>().SetStageData(PlayerPrefs.HasKey(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") ? (Difficulty)PlayerPrefs.GetInt(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") : Difficulty.Easy);
    }
}
