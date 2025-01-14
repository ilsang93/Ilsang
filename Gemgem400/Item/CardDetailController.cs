using GemgemAr;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 인벤토리에서 아이템 터치 시, 아이템 상세 정보 팝업을 제어하는 클래스
/// </summary>
public class CardDetailController : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemDetail;
    [SerializeField]
    private TextMeshProUGUI itemCount;
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private GameObject[] stars;
    [SerializeField]
    private GameObject newTag;
    [SerializeField]
    private Button submitButton;

    public void Start()
    {
        submitButton.onClick.AddListener(() =>
        {
            UnshowDetail();
        });
    }

    public void ShowDetail(CharacterData data, int cardCount)
    {
        // 초기화
        itemName.text = "";
        itemDetail.text = "";
        itemCount.text = "× 0";

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }

        itemName.text = data.Name;
        itemDetail.text = data.FlavorText;
        itemImage.sprite = data.Sprite;

        if (cardCount <= 0)
        {
            itemCount.text = "× 0";
        }
        else
        {
            itemCount.text = "× " + cardCount.ToString();
        }

        for (int i = 0; i < data.Level; i++)
        {
            stars[i].SetActive(true);
        }

        transform.DOLocalMove(Vector3.zero, 0.5f).SetEase(Ease.Linear);
    }

    public void UnshowDetail()
    {
        transform.DOLocalMove(new Vector3(0, -1080, 0), 0.5f).SetEase(Ease.Linear);
    }
}