using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

internal enum LobbyState
{
    None,
    ShortMenu,
    Booth
}
internal enum OnArea
{
    Stage,
    Setting,
    Store,
    Credits,
    Wardrobe,
    Composition,
    Exit,
    None
}

public class LobbyManager : MonoBehaviour
{
    [Header("Booth Position")]
    [SerializeField] private Vector3 stageBoothPos;
    [SerializeField] private Vector3 settingsBoothPos;
    [SerializeField] private Vector3 storeBoothPos;
    [SerializeField] private Vector3 exitBoothPos;
    [SerializeField] private Vector3 creditsBoothPos;
    [SerializeField] private Vector3 wardrobeBoothPos;
    [SerializeField] private Vector3 compositionBoothPos;

    [Header("Booth Description")]
    [SerializeField] internal DescriptionObjectController stageBoothDes;
    [SerializeField] internal DescriptionObjectController settingsBoothDes;
    [SerializeField] internal DescriptionObjectController storeBoothDes;
    [SerializeField] internal DescriptionObjectController exitBoothDes;
    [SerializeField] internal DescriptionObjectController creditsBoothDes;
    [SerializeField] internal DescriptionObjectController wardrobeBoothDes;
    [SerializeField] internal DescriptionObjectController compositionBoothDes;

    [Header("Booth Camera")]
    [SerializeField] private GameObject boothFollowObject;
    [SerializeField] private CinemachineCamera characterFollowCamera;
    [SerializeField] private CinemachineCamera boothCamera;
    [SerializeField] private CinemachineCamera waitCamera;

    internal static LobbyManager Instance;
    internal bool isRunning = false;
    internal LobbyState lobbyState = LobbyState.None;

    private LobbyPopupController lobbyPopupController;

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
        lobbyPopupController = GetComponent<LobbyPopupController>();
        ChangeCamera(false);
    }

    internal void DoShortMenu()
    {
        lobbyState = LobbyState.ShortMenu;
        isRunning = true;
        ShortMenuController.instance.DoShortMenu();
    }

    internal void OpenStage()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, stageBoothPos);
        StartCoroutine(SceneLoadManager.Instance.LoadScene("StageSelect"));
        // StartCoroutine(SceneLoadManager.Instance.LoadScene("GameStageBase"));
    }

    internal void OpenSetting()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, settingsBoothPos);
        lobbyPopupController.OpenPopup(OnArea.Setting);
    }

    internal void OpenStore()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, storeBoothPos);
        lobbyPopupController.OpenPopup(OnArea.Store);
    }

    internal void OpenCredits()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, creditsBoothPos);
        lobbyPopupController.OpenPopup(OnArea.Credits);
    }

    internal void OpenWardrobe()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, wardrobeBoothPos);
        lobbyPopupController.OpenPopup(OnArea.Wardrobe);
    }

    internal void OpenComposition()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, compositionBoothPos);
        lobbyPopupController.OpenPopup(OnArea.Composition);
    }

    internal void ExitGame()
    {
        if (lobbyState == LobbyState.Booth) return;
        DoBeforeBooth();
        ChangeCamera(true, exitBoothPos);

        CommonMessageManager.Instance.ShowMessage("게임을 종료하시겠습니까?", () =>
        {
            Application.Quit();
        }, MessageTypes.Confirm);

    }

    internal void CloseBooth()
    {
        lobbyState = LobbyState.None;
        lobbyPopupController.ClosePopup();

        ChangeCamera(false);
    }

    internal void ChangeCamera(bool boothFlg, Vector3 boothPos = new Vector3())
    {
        if (boothFlg)
        {
            boothCamera.Priority = 1;
            characterFollowCamera.Priority = 0;
        }
        else
        {
            boothCamera.Priority = 0;
            characterFollowCamera.Priority = 1;
        }
        boothFollowObject.transform.position = new Vector3(boothPos.x, boothPos.y, boothCamera.transform.position.z);
        StartCoroutine(CheckCameraMoving());
    }

    internal void ChangeCameraToWait()
    {
        boothCamera.Priority = 0;
        characterFollowCamera.Priority = 0;
        waitCamera.Priority = 1;
    }

    internal void ChangeCameraToCharacter()
    {
        boothCamera.Priority = 0;
        characterFollowCamera.Priority = 1;
        waitCamera.Priority = 0;
    }

    private void SelectInShortMenu()
    {
        ShortMenuController.instance.DoShortMenu();
        lobbyState = LobbyState.Booth;
    }

    private void DoBeforeBooth()
    {
        if (lobbyState == LobbyState.ShortMenu)
        {
            SelectInShortMenu();
        }

        lobbyState = LobbyState.Booth;
        isRunning = true;
    }

    private IEnumerator CheckCameraMoving()
    {
        while (true)
        {
            if (Vector3.Distance(boothCamera.transform.position, boothFollowObject.transform.position) <= 0.1f)
            {
                isRunning = false;
                yield break;
            }
            yield return null;
        }
    }
}