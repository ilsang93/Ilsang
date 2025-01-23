using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> introImages;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject introImageObj;
    [SerializeField] private GameObject autoSaveTextImg;
    [SerializeField] private GameObject autoSaveImg;
    [SerializeField] private GameObject PressAnyKeyText;
    [SerializeField] private GameObject GameTitleText;
    [SerializeField] private GameObject GameSubTitleText;
    [SerializeField] private GameObject GameTitleImg;
    [SerializeField] private GameObject CharacterPanelMask;
    [SerializeField] private GameObject CharacterPanel;
    // Start is called before the first frame update
    void Start()
    {
        introImageObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        autoSaveTextImg.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        autoSaveImg.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        PressAnyKeyText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
        StartCoroutine(Intro());
    }

    private IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < introImages.Count; i++)
        {
            introImageObj.GetComponent<Image>().sprite = introImages[i];
            introImageObj.GetComponent<Image>().SetNativeSize();
            introImageObj.GetComponent<Image>().DOFade(1, 1);
            yield return new WaitForSeconds(2);
            introImageObj.GetComponent<Image>().DOFade(0, 1);
            yield return new WaitForSeconds(1);
        }

        autoSaveTextImg.GetComponent<Image>().DOFade(1, 1);
        autoSaveImg.GetComponent<Image>().transform.DOLocalRotate(new Vector3(0, 0, 720), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutCirc);
        autoSaveImg.GetComponent<Image>().DOFade(1, 1);
        yield return new WaitForSeconds(6f);
        autoSaveTextImg.GetComponent<Image>().DOFade(0, 1);
        autoSaveImg.GetComponent<Image>().DOFade(0, 1);
        yield return new WaitForSeconds(1f);

        //TODO 데이터 로드 및 초기화가 완료 되면 (GameDataManager) 아래 코드 실행

        GameTitleText.GetComponent<TextMeshProUGUI>().transform.DOLocalMoveY(400, 1).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            GameTitleImg.GetComponent<Image>().DOFade(1, 1.5f).OnComplete(() =>
            {
                CharacterPanelMask.GetComponent<Image>().gameObject.SetActive(true);
                CharacterPanel.GetComponent<Image>().DOFade(1, 0);
                GameSubTitleText.GetComponent<TextMeshProUGUI>().DOFade(1, 0f);
            });
        });
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlaySFX("DM-CGS-10");
        yield return new WaitForSeconds(1.5f);
        PressAnyKeyText.GetComponent<TextMeshProUGUI>().DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
        SoundManager.Instance.PlayBGM("00");

        while (true)
        {
            if (Input.anyKeyDown)
            {
                break;
            }
            yield return null;
        }

        StartCoroutine(SceneLoadManager.Instance.LoadScene("Lobby"));
    }
}
