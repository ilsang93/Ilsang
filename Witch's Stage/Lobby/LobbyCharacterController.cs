using System;
using UnityEngine;

public class LobbyCharacterController : MonoBehaviour
{
    private LobbyCharacterInputManager inputManager;

    internal OnArea areaStatus = OnArea.None;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<LobbyCharacterInputManager>();


        inputManager.onPressEnter += () =>
        {
            OnPressEnter();
        };
        inputManager.onPressESC += () =>
        {
            if (LobbyManager.Instance.lobbyState == LobbyState.Booth)
            {
                LobbyManager.Instance.CloseBooth();
            }
            else
            {
                LobbyManager.Instance.DoShortMenu();
            }
        };

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LobbyArea"))
        {
            areaStatus = (OnArea)Enum.Parse(typeof(OnArea), other.name);

            switch (areaStatus)
            {
                case OnArea.Stage:
                    LobbyManager.Instance.stageBoothDes.OpenWindow();
                    break;
                case OnArea.Setting:
                    LobbyManager.Instance.settingsBoothDes.OpenWindow();
                    break;
                case OnArea.Store:
                    LobbyManager.Instance.storeBoothDes.OpenWindow();
                    break;
                case OnArea.Credits:
                    LobbyManager.Instance.creditsBoothDes.OpenWindow();
                    break;
                case OnArea.Wardrobe:
                    LobbyManager.Instance.wardrobeBoothDes.OpenWindow();
                    break;
                case OnArea.Composition:
                    LobbyManager.Instance.compositionBoothDes.OpenWindow();
                    break;
                case OnArea.Exit:
                    LobbyManager.Instance.exitBoothDes.OpenWindow();
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LobbyArea"))
        {
            switch (areaStatus)
            {
                case OnArea.Stage:
                    LobbyManager.Instance.stageBoothDes.CloseWindow();
                    break;
                case OnArea.Setting:
                    LobbyManager.Instance.settingsBoothDes.CloseWindow();
                    break;
                case OnArea.Store:
                    LobbyManager.Instance.storeBoothDes.CloseWindow();
                    break;
                case OnArea.Credits:
                    LobbyManager.Instance.creditsBoothDes.CloseWindow();
                    break;
                case OnArea.Wardrobe:
                    LobbyManager.Instance.wardrobeBoothDes.CloseWindow();
                    break;
                case OnArea.Composition:
                    LobbyManager.Instance.compositionBoothDes.CloseWindow();
                    break;
                case OnArea.Exit:
                    LobbyManager.Instance.exitBoothDes.CloseWindow();
                    break;
            }

            areaStatus = OnArea.None;
        }
    }

    private void OnPressEnter()
    {
        if (areaStatus == OnArea.None) return;

        switch (areaStatus)
        {
            case OnArea.Stage:
                LobbyManager.Instance.OpenStage();
                break;
            case OnArea.Setting:
                LobbyManager.Instance.OpenSetting();
                break;
            case OnArea.Store:
                LobbyManager.Instance.OpenStore();
                break;
            case OnArea.Credits:
                LobbyManager.Instance.OpenCredits();
                break;
            case OnArea.Wardrobe:
                LobbyManager.Instance.OpenWardrobe();
                break;
            case OnArea.Composition:
                LobbyManager.Instance.OpenComposition();
                break;
            case OnArea.Exit:
                LobbyManager.Instance.ExitGame();
                break;
        }
    }
}