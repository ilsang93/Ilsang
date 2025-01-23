using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonMessageManager : MonoBehaviour
{
    public static CommonMessageManager Instance { get; private set; }
    [SerializeField] private GameObject laycastMask;
    [SerializeField] private GameObject messageDialog;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private bool isShow = false;
    private MessageTypes nowMessageType;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        messageDialog.transform.localPosition = new Vector2(0, 800);
        laycastMask.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isShow)
            {
                switch (nowMessageType)
                {
                    case MessageTypes.Confirm:
                        cancelButton.onClick.Invoke();
                        break;
                    case MessageTypes.Info:
                        confirmButton.onClick.Invoke();
                        break;
                }
            }
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        MoveDialog(true);
    }

    public void ShowMessage(string message, Action onConfirm, MessageTypes messageType = MessageTypes.Info)
    {
        messageText.text = message;
        cancelButton.gameObject.SetActive(messageType == MessageTypes.Confirm);
        MoveDialog(true);
        isShow = true;

        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            onConfirm = null;
            MoveDialog(false);
        });

        switch (nowMessageType = messageType)
        {
            case MessageTypes.Info:
                break;
            case MessageTypes.Warning:
                break;
            case MessageTypes.Confirm:
                cancelButton.onClick.AddListener(() =>
                {
                    onConfirm = null;
                    ClickCancel();
                });
                break;
        }
    }

    internal void ClickCancel()
    {
        isShow = false;
        nowMessageType = MessageTypes.None;
        confirmButton.onClick.RemoveAllListeners();
        MoveDialog(false);
    }

    private void MoveDialog(bool isShow)
    {
        if (isShow)
        {
            laycastMask.SetActive(true);
            messageDialog.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            messageDialog.transform.DOLocalMoveY(800, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                laycastMask.SetActive(false);
            });
        }
    }


}

public enum MessageTypes
{
    Info,
    Warning,
    Confirm,
    None
}
