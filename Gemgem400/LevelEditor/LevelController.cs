using GemgemAr;

using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 레벨 에디터 화면을 제어하는 클래스
/// </summary>
public class LevelController : MonoBehaviour
{
    private const string JSON_ROUTE_PATH = "@@@경로@@@";
    private const string LEVEL_PROPERTIES_JSON_PATH = "@@@경로@@@";
    private const string MOTION_DATA_JSON_PATH = "@@@경로@@@";
    private const string GAME_LIST_DATA_JSON_PATH = "@@@경로@@@";
    [SerializeField] private GameObject levelScrollContent;
    [SerializeField] private GameObject stepScrollContent;
    [SerializeField] private GameObject detailScrollContent;
    [SerializeField] private GameObject diffScrollContent;
    [SerializeField] private Button levelBtnOrigin;
    [SerializeField] private Button stepBtnOrigin;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button updateBtn;
    [SerializeField] private Button stepDeleteBtn;
    [SerializeField] private Button stepTypeBtn;
    [SerializeField] private Button addStepBtn;
    [SerializeField] private Button backToStartBtn;
    [SerializeField] private Button diffBtn;
    [SerializeField] private Button initBtn;
    private JArray gameListData;
    private JObject levelData;
    public Dictionary<string, MotionModel> motionData = new Dictionary<string, MotionModel>();
    private string selectedDetail;
    private string selectedLevel;
    private int selectedStep;
    public static LevelController Instance { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (PlayerPrefs.HasKey("@@@레벨 커스텀 플래그@@@") && PlayerPrefs.GetString("@@@레벨 커스텀 플래그@@@").Equals("true"))
        {
            // 커스텀한 경우 PlayerPrefs에서 데이터를 가져온다.
            levelData = JObject.Parse(PlayerPrefs.GetString("@@@레벨 커스텀 데이터@@@"));
            initLevelList();
        }
        else
        {
            // 아닌경우 초기 json 데이터를 가져온다.
            AsyncOperationHandle<TextAsset> handle =
                Addressables.LoadAssetAsync<TextAsset>(JSON_ROUTE_PATH + LEVEL_PROPERTIES_JSON_PATH);
            handle.Completed += LoadLevelProperties;
        }

        LoadMotionData();

        AsyncOperationHandle<TextAsset> gameHandle =
            Addressables.LoadAssetAsync<TextAsset>(JSON_ROUTE_PATH + GAME_LIST_DATA_JSON_PATH);
        gameHandle.Completed += LoadGameList;

        saveBtn.onClick.AddListener(() =>
        {
            SaveLevelProperties();
        });

        initBtn.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("@@@레벨 커스텀 플래그@@@", "false");
            PlayerPrefs.DeleteKey("@@@레벨 커스텀 데이터@@@");
            PlayerPrefs.Save();
        });

        stepDeleteBtn.onClick.AddListener(() =>
        {
            DeleteStep();
        });

        stepTypeBtn.onClick.AddListener(() =>
        {
            ChangeStepType();
        });

        addStepBtn.onClick.AddListener(() =>
        {
            AddStep();
        });

        backToStartBtn.onClick.AddListener(() =>
        {
            Loading.SetTargetScene("@@@시작 메뉴@@@");
            Loading.Instance.LoadScene();
        });
    }

    private void initLevelList()
    {
        for (int i = 0; i < levelData.Count; i++)
        {
            int index = i + 1;
            Button levelBtn = Instantiate(levelBtnOrigin);
            levelBtn.transform.SetParent(levelScrollContent.transform, false);

            levelBtn.onClick.AddListener(() =>
            {
                initStepList(index.ToString());
                selectedLevel = index.ToString();
            });
            levelBtn.GetComponentInChildren<Text>().text = "레벨 " + index;
        }
    }

    private void initStepList(string level)
    {
        JArray thisLevelData = (JArray)levelData[level];

        for (int i = 0; i < stepScrollContent.transform.childCount; i++)
        {
            Destroy(stepScrollContent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < thisLevelData.Count; i++)
        {
            int index = i;
            Button stepBtn = Instantiate(stepBtnOrigin);
            stepBtn.transform.SetParent(stepScrollContent.transform, false);

            Debug.Log(thisLevelData[index]);
            Debug.Log(thisLevelData[index]["gameType"]);
            Debug.Log(thisLevelData[index]["gameType"].ToString().Equals("warmUp"));

            stepBtn.GetComponent<StepButtonManager>().gameType = thisLevelData[index]["gameType"].ToString();
            stepBtn.GetComponent<StepButtonManager>().gameId = thisLevelData[index]["gameId"].ToString();
            stepBtn.GetComponent<StepButtonManager>().difficulty = (int)thisLevelData[index]["difficulty"];
            stepBtn.GetComponent<StepButtonManager>().motionId = thisLevelData[index]["playingMotion"].ToString();
            stepBtn.GetComponent<StepButtonManager>().hand = thisLevelData[index]["playingHand"].ToString();

            stepBtn.GetComponent<StepButtonManager>().updateText();

            stepBtn.onClick.AddListener(() =>
            {
                noshowList();

                selectedStep = stepBtn.transform.GetSiblingIndex();
                stepDeleteBtn.gameObject.SetActive(true);
                stepTypeBtn.gameObject.SetActive(true);
                for (int i = 0; i < stepScrollContent.transform.childCount; i++)
                {
                    for (int j = 0; j < stepScrollContent.transform.GetChild(i).childCount; j++)
                    {
                        if (!(stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.name.Equals("TypeText") ||
                              stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.name.Equals("MotionText")))
                        {
                            stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                    }
                }

                for (int i = 0; i < stepBtn.transform.childCount; i++)
                {
                    stepBtn.transform.GetChild(i).gameObject.SetActive(true);
                }
            });
        }
    }

    private void DeleteStep()
    {
        Destroy(stepScrollContent.transform.GetChild(selectedStep).gameObject);
        noshowList();
    }

    private void AddStep()
    {
        Button stepBtn = Instantiate(stepBtnOrigin);
        stepBtn.transform.SetParent(stepScrollContent.transform, false);

        stepBtn.GetComponent<StepButtonManager>().gameType = "game";
        stepBtn.GetComponent<StepButtonManager>().gameId = "RobotFactory";
        stepBtn.GetComponent<StepButtonManager>().difficulty = 1;
        stepBtn.GetComponent<StepButtonManager>().motionId = "@@@동작 ID@@@";
        stepBtn.GetComponent<StepButtonManager>().hand = "right";

        stepBtn.GetComponent<StepButtonManager>().updateText();

        stepBtn.onClick.AddListener(() =>
        {
            noshowList();

            selectedStep = stepBtn.transform.GetSiblingIndex();
            stepDeleteBtn.gameObject.SetActive(true);
            stepTypeBtn.gameObject.SetActive(true);
            for (int i = 0; i < stepScrollContent.transform.childCount; i++)
            {
                for (int j = 0; j < stepScrollContent.transform.GetChild(i).childCount; j++)
                {
                    if (!(stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.name.Equals("TypeText") ||
                          stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.name.Equals("MotionText")))
                    {
                        stepScrollContent.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
                    }
                }
            }

            for (int i = 0; i < stepBtn.transform.childCount; i++)
            {
                stepBtn.transform.GetChild(i).gameObject.SetActive(true);
            }
        });

        noshowList();
    }

    private void ChangeStepType()
    {
        StepButtonManager step = stepScrollContent.transform.GetChild(selectedStep).gameObject
            .GetComponent<StepButtonManager>();
        if (step.gameType.Equals("game"))
        {
            step.gameType = "warmUp";
        }
        else if (step.gameType.Equals("warmUp"))
        {
            step.gameType = "stretch";
        }
        else
        {
            step.gameType = "game";
        }

        step.updateText();
    }

    private void LoadLevelProperties(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 로드된 JSON 파일의 텍스트 데이터를 가져옵니다.
            JsonWrapper jw = new();
            jw.jsonText = handle.Result.text;
            Addressables.Release(handle);

            levelData = JObject.Parse(jw.jsonText);

            initLevelList();
        }
        else
        {
            Debug.LogError("Failed to load JSON asset: " + handle.DebugName);
        }
    }

    private async void LoadMotionData()
    {
        var motionMessage = await GemgemApiService.GetMotions();
        var motions = await ApiHelper.ProcessResponse<List<MotionModel>>(motionMessage);
        
        if (motions != null)
        {
            // 로드된 JSON 파일의 텍스트 데이터를 가져옵니다.

            motionData = new Dictionary<string, MotionModel>();
            foreach (var motion in motions)
            {
                Debug.Log(motion.id);
                motionData.Add(motion.id, motion);    
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON");
        }
    }

    private void LoadGameList(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 로드된 JSON 파일의 텍스트 데이터를 가져옵니다.
            JsonWrapper jw = new()
            {
                jsonText = handle.Result.text
            };
            Addressables.Release(handle);

            Debug.Log("gameData " + jw.jsonText);
            gameListData = JArray.Parse(jw.jsonText);
        }
        else
        {
            Debug.LogError("Failed to load JSON asset: " + handle.DebugName);
        }
    }

    public void noshowList()
    {
        stepDeleteBtn.gameObject.SetActive(false);
        for (int i = 0; i < detailScrollContent.transform.childCount; i++)
        {
            Destroy(detailScrollContent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < diffScrollContent.transform.childCount; i++)
        {
            Destroy(diffScrollContent.transform.GetChild(i).gameObject);
        }
    }

    public void ShowGameList(int stepIndex, string settedGame)
    {
        noshowList();
        foreach (JObject game in gameListData.Cast<JObject>())
        {
            Button listBtn = Instantiate(levelBtnOrigin);
            listBtn.GetComponentInChildren<Text>().text = game["gameName"].ToString();
            listBtn.transform.SetParent(detailScrollContent.transform, false);

            listBtn.onClick.AddListener(() =>
            {
                stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().gameId = game["gameName"].ToString();
                for (int i = 0; i < 3; i++)
                {
                    Button diffBtn = Instantiate(this.diffBtn);
                    diffBtn.GetComponentInChildren<Text>().text = (i + 1).ToString();
                    diffBtn.transform.SetParent(diffScrollContent.transform, false);
                    
                    int difficulty = i;

                    diffBtn.onClick.AddListener(() =>
                    {
                        stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().difficulty = difficulty;
                        stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().updateText();
                    });
                }
                stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().updateText();
            });
        }
    }

    public void ShowMotionList(int stepIndex, string settedMotion)
    {
        noshowList();
        foreach (MotionModel motion in motionData.Values)
        {
            Button listBtn = Instantiate(levelBtnOrigin);
            listBtn.GetComponentInChildren<Text>().text = motion.id;
            listBtn.transform.SetParent(detailScrollContent.transform, false);

            listBtn.onClick.AddListener(() =>
            {
                stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().motionId = motion.id;
                stepScrollContent.transform.GetChild(stepIndex).GetComponent<StepButtonManager>().updateText();
            });
        }
    }

    private JObject MakeJsonData()
    {
        // level 데이터 생성

        JArray elements = new();
        for (int j = 0; j < stepScrollContent.transform.childCount; j++)
        {
            JObject element = new();
            StepButtonManager stepData = stepScrollContent.transform.GetChild(j).GetComponent<StepButtonManager>();

            element.Add("gameType", stepData.gameType);
            element.Add("gameId", stepData.gameId);
            element.Add("difficulty", stepData.difficulty);
            element.Add("played", "false");
            element.Add("playingMotion", stepData.motionId);
            element.Add("playingHand", stepData.hand);

            elements.Add(element);
        }

        levelData[selectedLevel] = elements;

        Debug.Log(levelData);
        return levelData;
    }

    private void SaveLevelProperties()
    {
        if (stepScrollContent.transform.childCount <= 0)
        {
            // msg : 스텝이 없는 상태에서는 레벨을 저장할 수 없습니다.
            return;
        }

        levelData = MakeJsonData();
        PlayerPrefs.SetString("@@@레벨 커스텀 플래그@@@", "true");
        PlayerPrefs.SetString("@@@레벨 커스텀 데이터@@@", levelData.ToString());
        PlayerPrefs.Save();
    }

    private IEnumerator ShowMsg(string text)
    {
        yield break;
    }
}