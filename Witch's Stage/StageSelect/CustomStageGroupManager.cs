using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class CustomStageGroupManager : MonoBehaviour
{
    [SerializeField] GameObject stageButtonPrefab;
    private readonly List<StageButtonController> stageButtonControllers = new();

    public int SelectedStageIndex { get => selectedStageIndex; set => selectedStageIndex = value; }
    private int selectedStageIndex;

    private string stageDataPath;

    private void Start()
    {
        stageDataPath = Application.persistentDataPath + StageConstant.STAGE_DATA_PATH;
        StartCoroutine(InitStageGroup());
    }

    public IEnumerator InitStageGroup()
    {
        StageData[] stageDatas = Resources.LoadAll<StageData>("Datas/StageDatas");

        for (int i = 0; i < stageDatas.Length; i++)
        {

            StageButtonController btnController = Instantiate(stageButtonPrefab, transform).GetComponent<StageButtonController>();
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

    public void LoadStageDatas()
    {
        DirectoryInfo di = new(stageDataPath);
        FileInfo[] files = di.GetFiles("*.json");

        foreach (FileInfo file in files)
        {
            string jsonData = File.ReadAllText(file.FullName);
            StageData tempData = JsonConvert.DeserializeObject<StageData>(jsonData);

            tempData.stageMusic = DataConvertUtils.ConvertBase64ToMusic(tempData.musicData);
            tempData.backgroundSprite = DataConvertUtils.ConvertBase64ToSprite(tempData.backgroundData);

            //TODO 스테이지 버튼을 추가하는 처리 필요
        }
    }
}
