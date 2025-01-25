using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGroupManager : MonoBehaviour
{
    [SerializeField] GameObject stageButtonPrefab;
    private readonly List<StageButtonController> stageButtonControllers = new();

    public int SelectedStageIndex { get => selectedStageIndex; set => selectedStageIndex = value; }
    private int selectedStageIndex;

    private void Start()
    {
        StartCoroutine(InitStageGroup());
    }

    public IEnumerator InitStageGroup()
    {
        StageData[] stageDatas = Resources.LoadAll<StageData>("Datas/StageDatas");

        for (int i = 0; i < stageDatas.Length; i++)
        {

            StageButtonController btnController = Instantiate(stageButtonPrefab, transform).GetComponent<StageButtonController>();
            btnController.onClicked += () => OnClickStageButton(btnController);
            stageButtonControllers.Add(btnController);

            btnController.InitButton(stageDatas[i]);
        }
        yield return new WaitForSeconds(0.1f);
        SelectedStageIndex = 0;
    }

    public void OnClickStageButton(StageButtonController stageButtonController)
    {
        foreach (StageButtonController controller in stageButtonControllers)
        {
            controller.DeselectButton();
        }
        stageButtonController.SelectButton();
        SelectedStageIndex = stageButtonControllers.IndexOf(stageButtonController);
    }
}
