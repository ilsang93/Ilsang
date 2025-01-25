using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ReadyController : MonoBehaviour
{
    [SerializeField]
    private GameObject readyBackground;
    [SerializeField]
    private GameObject readyText;
    [SerializeField]
    private GameObject readyPanel;

    // Start is called before the first frame update
    void Start()
    {
        readyBackground.transform.localScale = new Vector3(1, 0, 1);
        readyText.transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoReady()
    {
        StartCoroutine(ReadyGameCoroutine());
    }

    private IEnumerator ReadyGameCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        readyPanel.SetActive(true);
        readyBackground.SetActive(true);
        readyBackground.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);

        readyText.SetActive(true);
        readyText.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1.5f);

        readyBackground.transform.DOScaleY(0, 0.5f).SetEase(Ease.InSine);
        readyText.transform.DOScaleY(0, 0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.5f);

        readyBackground.GetComponent<Image>().color = Color.green;
        readyText.GetComponent<TextMeshProUGUI>().text = "Go!";
        readyText.GetComponent<TextMeshProUGUI>().fontSize = 120;
        readyBackground.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBounce);
        readyText.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);

        readyBackground.transform.DOScaleY(0, 0.5f).SetEase(Ease.InSine);
        readyText.transform.DOScaleY(0, 0.5f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(0.5f);

        readyPanel.SetActive(false);
        readyBackground.SetActive(false);
        readyText.SetActive(false);

        GameManager.instance.gameState = GameState.Play;
    }
}
