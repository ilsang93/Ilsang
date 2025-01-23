using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class LobbyPopupController : MonoBehaviour
{
    [Header("Popups")]
    [SerializeField] private LobbyPopupManager storePopup;
    [SerializeField] private LobbyPopupManager settingsPopup;
    [SerializeField] private LobbyPopupManager wardrobePopup;
    [SerializeField] private LobbyPopupManager compositionPopup;
    [SerializeField] private LobbyPopupManager creditsPopup;

    [Header("Popups Animation Positions")]
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float startRot;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private float endRot;

    private Dictionary<OnArea, LobbyPopupManager> popups = new();
    private OnArea popupArea;
    private bool isOpened = false;

    void Start()
    {
        popups.Add(OnArea.Store, storePopup);
        popups.Add(OnArea.Setting, settingsPopup);
        popups.Add(OnArea.Wardrobe, wardrobePopup);
        popups.Add(OnArea.Composition, compositionPopup);
        popups.Add(OnArea.Credits, creditsPopup);
    }

    internal void OpenPopup(OnArea area)
    {
        if (isOpened) return;

        LobbyPopupManager popup = popups[area];

        popup.transform.SetLocalPositionAndRotation(startPos, Quaternion.Euler(0, 0, startRot));

        popup.transform.DOLocalMove(Vector2.zero, 0.5f).SetEase(Ease.OutBack);
        popup.transform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutBack);

        // 팝업 연 후 처리
        switch (popupArea = area)
        {
            case OnArea.Store:
                break;
            default:
                break;
        }

        if (popupArea != OnArea.None) isOpened = true;
    }

    internal void ClosePopup()
    {
        if (!isOpened) return;

        LobbyPopupManager popup = popups[popupArea];

        popup.transform.DOLocalMove(endPos, 0.5f).SetEase(Ease.InBack);
        popup.transform.DOLocalRotate(new Vector3(0, 0, endRot), 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            popup.transform.SetLocalPositionAndRotation(popup.originPosition, Quaternion.identity);
        });

        // 팝업 닫은 후 처리
        switch (popupArea)
        {
            case OnArea.Store:
                break;
            default:
                break;
        }

        isOpened = false;
    }
}
