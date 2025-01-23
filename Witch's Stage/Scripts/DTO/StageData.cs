using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class StageData : ScriptableObject
{
    public string stageId;
    public string stageName;
    public string stageDescription;
    [JsonIgnore] public Sprite backgroundSprite;
    public int stageDifficulty;
    [JsonIgnore] public Color stageButtonColor;
    public JsonColor stageButtonColorVec3;

    [JsonIgnore] public Sprite jacketSprite;

    public LevelData[] levelData = new LevelData[4]; // Max 4

    [JsonIgnore] public TextAsset levelDataEasy;
    [JsonIgnore] public TextAsset levelDataNormal;
    [JsonIgnore] public TextAsset levelDataHard;
    [JsonIgnore] public TextAsset levelDataExtreme;

    public float time;
    [JsonIgnore] public AudioClip stageMusic;
    public string backgroundData;
    public string musicData;

    // 스테이지 작성 정보
    public string author;
    public string updateTime;
    public string musicName;

    [UnityEditor.MenuItem("Assets/Create/StageData")]
    public static void CreateTargetParamListAsset()
    {
        StageData asset = CreateDefaultInstance();
        UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/Datas/StageDatas/NewStageDatas.asset");
        UnityEditor.AssetDatabase.SaveAssets();
    }

    private static StageData CreateDefaultInstance()
    {
        var stageData = ScriptableObject.CreateInstance<StageData>();
        stageData.stageId = "";
        stageData.stageName = "";
        stageData.stageDescription = "";
        stageData.stageDifficulty = 0;
        stageData.stageButtonColorVec3 = new JsonColor(0, 0, 0, 1);
        stageData.time = 0;
        stageData.backgroundData = "";
        stageData.musicData = "";
        stageData.author = "";
        stageData.updateTime = "";
        stageData.musicName = "";

        for (int i = 0; i < 4; i++)
        {
            stageData.levelData[i] = new();
            stageData.levelData[i].diffIndex = i;
            stageData.levelData[i].level = 3 * (i + 1);
        }

        return stageData;
    }

    // 이전 JSON 내용을 저장
    private string cachedJsonEasy;
    private string cachedJsonNormal;
    private string cachedJsonHard;
    private string cachedJsonExtreme;

    // 에디터에서 값이 변경되었을 때 호출
    private void OnValidate()
    {
        if (levelDataEasy != null)
        {
            // JSON 파일 내용 읽기
            string jsonContent = levelDataEasy.text;

            // 기존 캐시와 비교하여 변경되었을 때만 실행
            if (jsonContent != cachedJsonEasy)
            {
                cachedJsonEasy = jsonContent;
                levelData[0] = ProcessJson(jsonContent);
            }
        }
        else
        {
            cachedJsonEasy = "";
        }

        if (levelDataNormal != null)
        {
            // JSON 파일 내용 읽기
            string jsonContent = levelDataNormal.text;

            // 기존 캐시와 비교하여 변경되었을 때만 실행
            if (jsonContent != cachedJsonNormal)
            {
                cachedJsonNormal = jsonContent;
                levelData[1] = ProcessJson(jsonContent);
            }
        }
        else
        {
            cachedJsonNormal = "";
        }

        if (levelDataHard != null)
        {
            // JSON 파일 내용 읽기
            string jsonContent = levelDataHard.text;

            // 기존 캐시와 비교하여 변경되었을 때만 실행
            if (jsonContent != cachedJsonHard)
            {
                cachedJsonHard = jsonContent;
                levelData[2] = ProcessJson(jsonContent);
            }
        }
        else
        {
            cachedJsonHard = "";
        }

        if (levelDataExtreme != null)
        {
            // JSON 파일 내용 읽기
            string jsonContent = levelDataExtreme.text;

            // 기존 캐시와 비교하여 변경되었을 때만 실행
            if (jsonContent != cachedJsonExtreme)
            {
                cachedJsonExtreme = jsonContent;
                levelData[3] = ProcessJson(jsonContent);
            }
        }
        else
        {
            cachedJsonExtreme = "";
        }
    }

    // JSON 처리 메서드
    private LevelData ProcessJson(string jsonContent)
    {
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(jsonContent);

        levelData.nodeList = levelData.jsonNodeList.ConvertAll(v => v.ToVector2());

        return levelData;
    }
}