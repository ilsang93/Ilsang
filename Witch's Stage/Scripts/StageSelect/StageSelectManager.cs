using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public static StageSelectManager Instance;
    [HideInInspector] public StageSelectInputManager inputManager;

    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI userLevelText;
    [SerializeField] private Image userProfileImage;

    public string UserName { set => userNameText.text = value; }
    public int UserLevel { set => userLevelText.text = "LV." + value.ToString(); }
    public Sprite UserProfileImage { set => userProfileImage.sprite = value; }

    public StageData selectedStageData;
    public StageSelectState stageSelectState = StageSelectState.Normal;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        inputManager = GetComponent<StageSelectInputManager>();
        // 테스트용 코드
        if (!GameDataManager.instance)
        {
            UserName = "TestUser";
            UserLevel = 1;
            UserProfileImage = Resources.Load<Sprite>("Images/ProfileImage/ProfileImage_1");
        }
        else
        {
            UserName = GameDataManager.instance.UserData.userName;
            UserLevel = GameDataManager.instance.UserData.userLevel;
            UserProfileImage = GameDataManager.instance.UserData.userProfileImage;
        }
    }

    public void OnClickPlayButton()
    {

        //TODO 스테이지 확인 절차를 추가한다.
        TransferStageData trasferStageData = new(
            selectedStageData,
            FindObjectOfType<DifficultySliderController>().Difficulty,
            FindObjectOfType<EffectGroupController>().nowSpeed
            );

        // 스테이지를 로드한다.
        StartCoroutine(SceneLoadManager.Instance.LoadStageScene(trasferStageData));
    }
}

public enum StageSelectState
{
    Normal, // 일반 상태
    Message, // 메시지 출력 상태
    Working // 작동 중 상태
}