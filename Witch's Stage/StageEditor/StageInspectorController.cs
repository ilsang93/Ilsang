using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInspectorController : MonoBehaviour
{
    public static StageInspectorController Instance { get; private set; }

    [Header("Stage Inspector")]
    [SerializeField] private Image sampleDisk;
    public TMP_InputField stageID;
    public TMP_InputField stageName;
    public TextMeshProUGUI stageMusic;
    public Slider red;
    public Slider green;
    public Slider blue;
    public TMP_InputField stageDescription;
    public TextMeshProUGUI updateTime;
    public List<Button> DifficultyDisks;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button selectMusicButton;
    [SerializeField] private Button selectBackgroundButton;
    [SerializeField] private GameObject musicSelectDialog;
    [SerializeField] private GameObject backgroundSelectDialog;

    // outside label
    [Header("Outside Inspector")]
    [SerializeField] private TextMeshProUGUI outStageID;
    [SerializeField] private TextMeshProUGUI outStageName;
    [SerializeField] private TextMeshProUGUI outStageMusic;

    public int StageDifficulty
    {
        get
        {
            return stageDifficulty;
        }
        set
        {
            stageDifficulty = value;
            for (int i = 0; i < DifficultyDisks.Count; i++)
            {
                DifficultyDisks[i].image.color = new Color(0, 0, 0);
            }
            for (int i = 0; i < stageDifficulty; i++)
            {
                DifficultyDisks[i].image.color = new Color(1, 1, 1);
            }
        }
    }
    private int stageDifficulty;
    private bool isLoading = false;

    private void Awake()
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
    private void Start()
    {
        sampleDisk.color = new Color(red.value, green.value, blue.value);

        red.onValueChanged.AddListener((value) =>
        {
            if (isLoading) return;
            sampleDisk.color = new Color(red.value, green.value, blue.value);
        });
        green.onValueChanged.AddListener((value) =>
        {
            if (isLoading) return;
            sampleDisk.color = new Color(red.value, green.value, blue.value);
        });
        blue.onValueChanged.AddListener((value) =>
        {
            if (isLoading) return;
            sampleDisk.color = new Color(red.value, green.value, blue.value);
        });

        foreach (var button in DifficultyDisks)
        {
            button.onClick.AddListener(() =>
            {
                StageDifficulty = DifficultyDisks.IndexOf(button) + 1;
            });
        }

        selectMusicButton.onClick.AddListener(SelectMusic);
        selectBackgroundButton.onClick.AddListener(SelectBackground);
        saveButton.onClick.AddListener(SaveSettings);
        cancelButton.onClick.AddListener(CancelSettings);
        closeButton.onClick.AddListener(CancelSettings);

        StageEditorManager.Instance.OnStageDataLoaded += OnLoadData;
    }

    private void SelectMusic()
    {
        musicSelectDialog.transform.localScale = Vector3.one;
    }

    private void SelectBackground()
    {
        backgroundSelectDialog.transform.localScale = Vector3.one;
    }

    private void OnLoadData()
    {
        isLoading = true;
        if (StageEditorManager.Instance.stageData)
        {
            stageID.text = StageEditorManager.Instance.stageData.stageId;
            stageName.text = StageEditorManager.Instance.stageData.stageName;
            stageDescription.text = StageEditorManager.Instance.stageData.stageDescription;
            stageMusic.text = StageEditorManager.Instance.stageData.musicName;
            sampleDisk.color = StageEditorManager.Instance.stageData.stageButtonColorVec3.ToColor();

            red.value = sampleDisk.color.r;
            green.value = sampleDisk.color.g;
            blue.value = sampleDisk.color.b;

            StageDifficulty = StageEditorManager.Instance.stageData.stageDifficulty;

            outStageMusic.text = StageEditorManager.Instance.stageData.musicName;
            outStageID.text = StageEditorManager.Instance.stageData.stageId;
            outStageName.text = StageEditorManager.Instance.stageData.stageName;
        }
        isLoading = false;
    }

    private void SaveSettings()
    {
        if (!StageEditorManager.Instance.stageData)
        {
            StageEditorManager.Instance.stageData = ScriptableObject.CreateInstance<StageData>();
        }

        StageEditorManager.Instance.stageData.stageId = stageID.text;
        StageEditorManager.Instance.stageData.stageName = stageName.text;
        StageEditorManager.Instance.stageData.stageDescription = stageDescription.text;
        StageEditorManager.Instance.stageData.stageButtonColor = sampleDisk.color;
        StageEditorManager.Instance.stageData.stageDifficulty = StageDifficulty;
        StageEditorManager.Instance.stageData.musicName = stageMusic.text;
        MusicSelectDialogController.Instance.beforeMusicDatas = MusicSelectDialogController.Instance.nowMusicDatas;
        BackgroundSelectController.Instance.beforeSprite = BackgroundSelectController.Instance.nowSprite;

        StageEditorManager.Instance.SetMusic(MusicSelectDialogController.Instance.nowMusicDatas);
        StageEditorManager.Instance.SetBackground(BackgroundSelectController.Instance.nowSprite);

        outStageID.text = stageID.text;
        outStageName.text = stageName.text;
        outStageMusic.text = stageMusic.text;

        transform.localScale = Vector3.zero;
        StageEditorManager.Instance.isOtherProcess = false;
    }

    private void CancelSettings()
    {
        if (StageEditorManager.Instance.stageData)
        {
            stageID.text = StageEditorManager.Instance.stageData.stageId;
            stageName.text = StageEditorManager.Instance.stageData.stageName;
            stageDescription.text = StageEditorManager.Instance.stageData.stageDescription;
            sampleDisk.color = StageEditorManager.Instance.stageData.stageButtonColor;

            MusicSelectDialogController.Instance.nowMusicDatas = MusicSelectDialogController.Instance.beforeMusicDatas;
            MusicSelectDialogController.Instance.beforeMusicDatas = null;

            BackgroundSelectController.Instance.nowSprite = BackgroundSelectController.Instance.beforeSprite;
            BackgroundSelectController.Instance.beforeSprite = null;

            red.value = sampleDisk.color.r;
            green.value = sampleDisk.color.g;
            blue.value = sampleDisk.color.b;

            StageDifficulty = StageEditorManager.Instance.stageData.stageDifficulty;
        }

        transform.localScale = Vector3.zero;
        StageEditorManager.Instance.isOtherProcess = false;
    }
}
