using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ShortMenuController : MonoBehaviour
{
    public static ShortMenuController instance;

    [SerializeField] private Button stage;
    [SerializeField] private Button setting;
    [SerializeField] private Button store;
    [SerializeField] private Button wardrobe;
    [SerializeField] private Button network;
    [SerializeField] private Button exit;

    private RectTransform rectTransform;
    private bool isRunning = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AddEvent();
    }

    void Update()
    {

    }

    public void DoShortMenu()
    {
        if (!isRunning)
        {
            if (rectTransform.anchoredPosition.x == 0)
            {
                HideShortMenu();
            }
            else
            {
                ShowShortMenu();
            }
        }
    }

    private void ShowShortMenu()
    {
        isRunning = true;
        rectTransform.DOAnchorPosX(0, 0.5f).onComplete += () => { isRunning = false; LobbyManager.Instance.isRunning = false; };
    }

    private void HideShortMenu()
    {
        isRunning = true;
        rectTransform.DOAnchorPosX(-rectTransform.sizeDelta.x, 0.5f).OnComplete(() => { isRunning = false; LobbyManager.Instance.isRunning = false; });
    }

    private void AddEvent()
    {
        stage.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.OpenStage();
        });

        setting.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.OpenSetting();
        });

        store.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.OpenStore();
        });

        wardrobe.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.OpenWardrobe();
        });

        network.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.OpenComposition();
        });

        exit.onClick.AddListener(() =>
        {
            HideShortMenu();
            LobbyManager.Instance.ExitGame();
        });
    }
}
