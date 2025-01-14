using TMPro;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using GemgemAr;
using Universal.UniversalSDK;

/// <summary>
/// 범용 메세지를 제어하는 클래스
/// </summary>
public class MessageController : MonoBehaviour
{
    [SerializeField] private GameObject messageObj;
    [SerializeField] private TextMeshProUGUI headLineText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GridLayoutGroup confirmBtnsGrid;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button getEggConfirmBtn;
    [SerializeField] private GameObject raycastBlocker;
    [SerializeField] private GameObject getEggMsgObj;
    [SerializeField] private Canvas msgCanvas;
    public static MessageController Instance { get; private set; }
    public Action onConfirmBtnClick;

    public string InputtedText
    {
        get
        {
            if (_inputField == null || _inputField.text == null)
            {
                return "";
            }
            else
            {
                return _inputField.text;
            }
        }
    }

    private Vector3 headerPos;
    private Vector3 messagePos;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        headerPos = headLineText.transform.localPosition;
        messagePos = messageText.transform.localPosition;

        confirmBtn.onClick.AddListener(() =>
        {
            Action tempAction = onConfirmBtnClick;
            onConfirmBtnClick = null;
            messageObj.transform.DOLocalMove(new Vector3(0, 1080, 0), 0.5f, true).OnComplete(() =>
            {
                messageText.text = "";
                cancelBtn.gameObject.SetActive(false);
                confirmBtnsGrid.cellSize = new Vector2(900, 100);
                tempAction?.Invoke();
                raycastBlocker.SetActive(false);
            });
        });

        cancelBtn.onClick.AddListener(() =>
        {
            messageObj.transform.DOLocalMove(new Vector3(0, 1080, 0), 0.5f, true).OnComplete(() =>
            {
                messageText.text = "";
                cancelBtn.gameObject.SetActive(false);
                confirmBtnsGrid.cellSize = new Vector2(900, 100);
                onConfirmBtnClick = null;
                raycastBlocker.SetActive(false);
            });
        });
    }

    void Update()
    {
        if (!msgCanvas.worldCamera)
        {
            if (GameObject.Find("@@@오버레이 카메라@@@"))
            {
                msgCanvas.worldCamera = GameObject.Find("@@@오버레이 카메라@@@").GetComponent<Camera>();
            }
            else
            {
                msgCanvas.worldCamera = GameObject.Find("@@@메인 카메라@@@").GetComponent<Camera>();
            }
        }
    }

    void SetupMessageBox(MessageData msgData)
    {
        switch (msgData.type)
        {
            case MessageType.Notice:
                headLineText.gameObject.SetActive(true);
                _inputField.gameObject.SetActive(false);
                headLineText.text = "알림";
                break;
            case MessageType.Error:
                headLineText.gameObject.SetActive(true);
                _inputField.gameObject.SetActive(false);
                headLineText.text = "오류";
                break;
            case MessageType.Confirm:
                headLineText.gameObject.SetActive(true);
                _inputField.gameObject.SetActive(false);
                headLineText.text = "확인";
                break;
            case MessageType.TextInput:
                headLineText.gameObject.SetActive(false);
                _inputField.gameObject.SetActive(true);
                _inputField.text = "";
                break;
        }

        if (msgData.type.Equals(MessageType.Confirm) || msgData.type.Equals(MessageType.TextInput))
        {
            cancelBtn.gameObject.SetActive(true);
            confirmBtnsGrid.cellSize = new Vector2(450, 100);
        }
        else
        {
            cancelBtn.gameObject.SetActive(false);
            confirmBtnsGrid.cellSize = new Vector2(900, 100);
        }

        if (msgData.type == MessageType.TextInput)
        {
            messageText.transform.localPosition = headerPos;
        }
        else
        {
            headLineText.transform.localPosition = headerPos;
            messageText.transform.localPosition = messagePos;
        }

        messageText.text = msgData.msg;
        messageObj.transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f, true).OnComplete(() =>
        {
            raycastBlocker.SetActive(true);
        });
        raycastBlocker.SetActive(true);
    }
    public bool DoInternetCheck()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            onConfirmBtnClick += (() =>
            {
                Application.Quit();
            });
            DoPopup(MessageIdx.message0006);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DoPopup(MessageIdx idx)
    {
        MessageData msgData = MessageIndex[idx];
        SetupMessageBox(msgData);
    }

    // 동적인 메시지 때문에 미리 정의해둘 수 없는 경우에 사용
    public void DoPopup(string msg)
    {
        SetupMessageBox(new MessageData(MessageIdx.message0000, msg, MessageType.Notice));
        raycastBlocker.SetActive(true);
    }

    public void DoWithdrawalPopup()
    {
        SetupMessageBox(MessageIndex[MessageIdx.message0038]);
        headLineText.text = "계정을 삭제하시겠습니까?";
        onConfirmBtnClick += (async () =>
        {
            onConfirmBtnClick = null;
            DoPopup(MessageIdx.message0048);
            raycastBlocker.SetActive(true);
            onConfirmBtnClick += async () =>
            {
                if (InputtedText == "삭제하겠습니다")
                {
                    var user = await UserApiService.UpdateUserWithdrawal();
                    if (user != null)
                    {
                        UniversalSDK.Ins.Logout();
                        PlayerPrefs.DeleteAll();
                        print("계정 삭제 완료");
                        DoPopup(MessageIdx.message0040);
                        onConfirmBtnClick += (() =>
                        {
                            Application.Quit();
                        });
                        raycastBlocker.SetActive(true);
                    }
                }
                else
                {
                    DoPopup(MessageIdx.message0049);
                    raycastBlocker.SetActive(true);
                }
            };

        });
        raycastBlocker.SetActive(true);
    }

    /// <summary>
    /// 범용 에러 메세지 팝업 
    /// </summary>
    /// <param name="errMsg"></param>
    public void DoErrPopup(string errMsg)
    {
        SetupMessageBox(new MessageData(MessageIdx.message0000, errMsg, MessageType.Error));
        messageText.text = MessageIndex[MessageIdx.message0000] + " : " + errMsg;
        raycastBlocker.SetActive(true);
    }

    public static Dictionary<MessageIdx, MessageData> MessageIndex = new()
    {
        {MessageIdx.message0000,        new MessageData(MessageIdx.message0000,         "오류가 발생했습니다. 관리자에게 문의해 주세요.", MessageType.Error)},
        //@@@이하 메세지 상세 데이터는 생략합니다@@@
    };

    public enum MessageIdx
    {
        message0000,
        message0001,
        message0002,
        message0003,
        message0004,
        message0005,
        message0006,
        message0007,
        message0008,
        message0009,
        message0010,
        message0011,
        message0012,
        message0013,
        message0014,
        message0015,
        message0016,
        message0017,
        message0018,
        message0019,
        message0020,
        message0021,
        message0022,
        message0023,
        message0024,
        message0025,
        message0026,
        message0027,
        message0029,
        message0030,
        message0031,
        message0032,
        message0033,
        message0034,
        message0035,
        message0036,
        message0037,
        message0038,
        message0039,
        message0040,
        message0041,
        message0042,
        message0043,
        message0044,
        message0045,
        message0046,
        message0047,
        message0048,
        message0049,
        message0050,
        message0051,
        message0052,
        debugmessage0001,
        debugmessage0002,
    }

    public class MessageData
    {
        public MessageIdx idx;
        public string msg;
        public MessageType type;

        public MessageData(MessageIdx idx, string msg, MessageType type)
        {
            this.idx = idx;
            this.msg = msg;
            this.type = type;
        }
    }

    public enum MessageType
    {
        Notice,
        Error,
        Confirm,
        TextInput
    }
}