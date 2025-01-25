using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AsyncLoadPanelManager : MonoBehaviour
{
    public static AsyncLoadPanelManager instance;
    [SerializeField] private Image asyncLoadIcon;

    void Awake()
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

    void Start()
    {
        asyncLoadIcon.color = new Color(1, 1, 1, 0);
        asyncLoadIcon.gameObject.SetActive(false);
    }

    internal void ShowAsyncLoadIcon()
    {
        asyncLoadIcon.gameObject.SetActive(true);
        asyncLoadIcon.transform.DOLocalRotate(new Vector3(0, 0, 720), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutCirc);
        asyncLoadIcon.DOColor(new Color(1, 1, 1, 1), 0.5f);
    }

    internal void HideAsyncLoadIcon()
    {
        asyncLoadIcon.DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(() =>
        {
            asyncLoadIcon.gameObject.SetActive(false);
        });
    }
}
