using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;

public class StageDataManager : MonoBehaviour
{
    public static StageDataManager Instance { get; private set; }

    [SerializeField] private TMP_InputField StageID;
    [SerializeField] private TMP_InputField StageName;

    private string stageDataPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        stageDataPath = Application.persistentDataPath + StageConstant.STAGE_DATA_PATH;

        if (!Directory.Exists(stageDataPath))
        {
            Directory.CreateDirectory(stageDataPath);
        }

        FindObjectOfType<SaveButtonController>().OnSaveButtonClicked += GenerateStageData;
        FindObjectOfType<LoadButtonController>().OnLoadButtonClicked += LoadStageDatas;
    }

    private bool VerifiyStageData()
    {
        bool isDataValid = true;

        if (string.IsNullOrEmpty(StageName.text))
        {
            throw new Exception("스테이지 이름을 입력해주세요.");
        }
        if (StageEditorManager.Instance.nodeList.Count == 0)
        {
            throw new Exception("노드가 없습니다.");
        }
        if (StageEditorManager.Instance.noteList.Count == 0)
        {
            throw new Exception("노트가 없습니다.");
        }
        if (StageEditorManager.Instance.stageMusic == null)
        {
            throw new Exception("선택한 음악이 없습니다.");
        }

        return isDataValid;
    }

    public void GenerateStageData()
    {
        try
        {
            if (!VerifiyStageData()) return;

            StageData tempData = ScriptableObject.CreateInstance(typeof(StageData)) as StageData;
            // 스테이지 ID
            // 기존 스테이지인 경우, 스테이지 ID를 그대로 유지한다.
            // 신규 스테이지인 경우 공백으로 지정한다. 이 값은 서버에서 관리한다.
            tempData.stageId = string.IsNullOrEmpty(StageID.text) ? "" : StageID.text;
            // 스테이지 이름
            tempData.stageName = StageName.text;
            // 스테이지 설명
            tempData.stageDescription = StageInspectorController.Instance.stageDescription.text;
            // 스테이지 배경 이미지 경로        
            tempData.backgroundData = "";
            // 스테이지 난이도
            tempData.stageDifficulty = 7;
            // 스테이지 버튼 색상
            tempData.stageButtonColorVec3 = new JsonColor(StageInspectorController.Instance.red.value, StageInspectorController.Instance.green.value, StageInspectorController.Instance.blue.value, 1);

            tempData.time = StageEditorManager.Instance.stageMusic.length;

            // 노드 리스트
            List<JsonVector2> nodeVecList = new();
            foreach (GameObject node in StageEditorManager.Instance.nodeList)
            {
                nodeVecList.Add(new JsonVector2(node.transform.position));
            }
            tempData.levelData[0].jsonNodeList = nodeVecList;

            // 노트 리스트
            List<NoteData> targetTimeList = new();
            foreach (GameObject note in StageEditorManager.Instance.noteList)
            {
                targetTimeList.Add(new NoteData()
                {
                    time = note.GetComponent<EditorNoteController>().targetTime
                });
            }
            tempData.levelData[0].noteList = targetTimeList;

            // 음악
            tempData.musicData = DataConvertUtils.ConvertMusicToBase64(StageEditorManager.Instance.stageMusic);
            tempData.musicName = StageEditorManager.Instance.stageMusic.name;

            // 배경 이미지
            tempData.backgroundSprite = BackgroundSelectController.Instance.nowSprite;
            tempData.backgroundData = DataConvertUtils.ConvertSpriteToBase64(BackgroundSelectController.Instance.nowSprite);

            SaveStageData(tempData);

            CommonMessageManager.Instance.ShowMessage("스테이지 데이터가 저장되었습니다.");
        }
        catch (Exception e)
        {
            CommonMessageManager.Instance.ShowMessage(e.Message);
            print(e.Message);
        }
    }

    public void SaveStageData(StageData stageData)
    {
        // 데이터 저장
        string path = Path.Combine(stageDataPath, stageData.stageName + ".json");
        string jsonData = JsonConvert.SerializeObject(stageData);
        File.WriteAllText(path, jsonData);
    }

    public void LoadStageDatas()
    {
        DirectoryInfo di = new(stageDataPath);
        if (!di.Exists) di.Create();
        FileInfo[] files = di.GetFiles("*.json");

        DataLoadDialogManager.Instance.ResetAllData();

        foreach (FileInfo file in files)
        {
            string jsonData = File.ReadAllText(file.FullName);
            StageData tempData = JsonConvert.DeserializeObject<StageData>(jsonData);

            tempData.stageMusic = DataConvertUtils.ConvertBase64ToMusic(tempData.musicData);
            tempData.backgroundSprite = DataConvertUtils.ConvertBase64ToSprite(tempData.backgroundData);

            DataLoadDialogManager.Instance.AddData(tempData);
        }
    }
}
