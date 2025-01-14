using DG.Tweening;

using GemgemAr;

using SRF;

using System.Collections;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour
{
    [SerializeField]
    private Image[] starArray = new Image[5];
    [SerializeField]
    private Image characterImage;
    [SerializeField]
    private TextMeshProUGUI characterName;
    [SerializeField]
    private Sprite[] cardBackgrounds;
    [SerializeField]
    private Image characterBackground;
    [SerializeField]
    private TextMeshProUGUI cardCountText;

    private CharacterData _characterData;
    public CharacterData CharacterData { get => _characterData; set => _characterData = value; }
    private Image cardBackground;
    private int _cardCount;
    public int CardCount
    {
        get { return _cardCount; }
        set
        {
            _cardCount = value;
            if (_cardCount <= 0)
            {
                cardCountText.text = "× 0";
            }
            else
            {
                cardCountText.text = "× " + _cardCount.ToString();
            }
        }
    }

    public void Init(CharacterData characterData)
    {
        _characterData = characterData;
        Debug.Log("캐릭터 데이터 로드 : " + _characterData);
        cardBackground = GetComponent<Image>();

        // 캐릭터 일러스트를 설정한다.
        characterImage.sprite = _characterData.Sprite;
        // 캐릭터 이름을 설정한다.
        characterName.text = _characterData.Name;
        // 캐릭터의 퍼스널 컬러를 설정한다.
        characterBackground.color = _characterData.PersonalColor;
        // 카드 배경을 설정한다.
        cardBackground.sprite = cardBackgrounds[_characterData.Level];
        for (int i = 0; i < _characterData.Level; i++)
        {
            starArray[i].gameObject.SetActive(true);
        }
    }

    public void SetStarPos(bool useEffect = true)
    {
        // 레벨만큼의 별을 활성화한다.
        if (_characterData != null)
        {
            if (useEffect == true)
            {
                StartCoroutine(CR_SetStarPos());    
            }
            else
            {
                starArray[0].transform.parent.gameObject.SetActive(true);
                for (int i = 0; i < _characterData.Level; i++)
                {
                    starArray[i].gameObject.SetActive(true);
                }
            }
            
        }
    }

    private IEnumerator CR_SetStarPos()
    {

        float yScreenHalf = Camera.main.orthographicSize;
        float xScreenHalf = yScreenHalf * Camera.main.aspect;

        GridLayoutGroup layoutGroup = starArray[0].transform.parent.GetComponent<GridLayoutGroup>();
        Vector3 startPos = layoutGroup.transform.position;
        Vector3 startSclae = layoutGroup.transform.localScale;
        starArray[0].transform.parent.gameObject.SetActive(true);
        for (int i = 0; i < starArray.Count(); i++)
        {
            starArray[i].gameObject.SetActive(false);
        }
        layoutGroup.transform.localScale *= 4.0f;
        layoutGroup.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width * -0.5f, Screen.height)).y,
            Camera.main.transform.position.z);
        layoutGroup.transform.DOMove(startPos, 1.0f);
        layoutGroup.transform.DOScale(startSclae, 1.0f);
        for (int i = 0; i < _characterData.Level; i++)
        {
            starArray[i].gameObject.SetActive(true);
            starArray[i].transform.DORotate(new Vector3(0, 0, 360), 1.0f, RotateMode.FastBeyond360);
        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < _characterData.Level; i++)
        {
            starArray[i].transform.DOJump(starArray[i].transform.position, 0.5f, 1, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
        layoutGroup.transform.DOScale(startSclae * 2.0f, 0.1f);
        yield return new WaitForSeconds(0.1f);
        layoutGroup.transform.DOScale(startSclae, 0.4f);
        yield return new WaitForSeconds(0.4f);
    }

    private void OnClickCard() {
        CardDetailController detailObj = FindObjectOfType<CardDetailController>();
    }
}