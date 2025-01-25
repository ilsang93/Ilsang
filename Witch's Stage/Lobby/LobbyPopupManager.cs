using UnityEngine;
using UnityEngine.UI;

public class LobbyPopupManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    [HideInInspector] public Vector3 originPosition;
    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.localPosition;

        closeButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CloseBooth();
        });
    }
}
