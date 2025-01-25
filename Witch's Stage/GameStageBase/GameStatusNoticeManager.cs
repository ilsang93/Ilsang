using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Unity.Cinemachine;

public class GameStatusNoticeManager : MonoBehaviour
{
    [SerializeField] private GameObject gsnArea;
    [SerializeField] private Image gsnPanel;
    [SerializeField] private Image gsnBar;
    [SerializeField] private TextMeshProUGUI gsnText;
    [SerializeField] private GameObject tensionObj;
    [SerializeField] private GameObject nodeActiveEffect;
    [SerializeField] private CinemachineCamera cameraObj;

    private const string READY_TEXT_VAL = "READY";
    private const string START_TEXT_VAL = "GO!";
    private const string OVER_TEXT_VAL = "GAME OVER";
    private const string CLEAR_TEXT_VAL = "STAGE CLEAR!";

    private SpriteRenderer character;
    private SpriteRenderer trailer;
    private List<GameObject> railRoadList;
    private List<GameObject> nodeList;

    void Start()
    {
        gsnArea.SetActive(true);
    }

    /// <summary>
    /// 오브젝트 초기화
    /// </summary>
    public void InitOnStart(List<GameObject> railRoads, List<GameObject> nodes)
    {
        gsnPanel.color = new Color32(0, 0, 0, 255);
        gsnBar.color = new Color32(60, 200, 60, 120);

        gsnPanel.rectTransform.localScale = Vector3.one;
        gsnBar.rectTransform.localScale = new Vector3(1, 0, 1);

        character = GameObject.Find("CharacterSprite").GetComponent<SpriteRenderer>();
        trailer = GameObject.Find("Spotlight").GetComponent<SpriteRenderer>();

        railRoadList = railRoads;
        nodeList = nodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            railRoadList[i].SetActive(false);
            nodeList[i].SetActive(false);
        }

        cameraObj.Lens.OrthographicSize = 80f;

        character.color = StageConstant.COLOR32_BLACK;
        trailer.color = StageConstant.COLOR32_CLEAR;
        tensionObj.SetActive(false);

        gsnText.text = READY_TEXT_VAL;

        gsnArea.SetActive(true);
    }

    /// <summary>
    /// 게임 시작 시의 연출 처리
    /// </summary>
    public Sequence GameStart()
    {
        Sequence seq = DOTween.Sequence();

        // 캐릭터, 저지컨트롤러, 노드 연출출 초기화
        seq.Append(gsnBar.rectTransform.DOScaleY(1, 0.3f).SetEase(Ease.OutBounce));
        seq.AppendInterval(0.5f);
        seq.Append(gsnBar.rectTransform.DOScaleY(0, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendCallback(() =>
        {
            gsnText.text = START_TEXT_VAL;
            gsnBar.color = new Color32(255, 130, 150, 120);
        });
        seq.AppendInterval(0.3f);

        // 스포트 라이트 연출 캐릭터와 저지 트레일러 표시
        seq.AppendCallback(() =>
        {
            trailer.color = StageConstant.COLOR32_SPOTLIGHT_1;
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            trailer.color = StageConstant.COLOR32_SPOTLIGHT_2;
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            trailer.color = StageConstant.COLOR32_SPOTLIGHT_3;
            character.color = StageConstant.COLOR32_WHITE;
        });
        seq.AppendInterval(0.5f);
        seq.Append(trailer.DOColor(StageConstant.COLOR32_SAFEAREA, 1f));
        seq.AppendCallback(() =>
        {
            tensionObj.SetActive(true);
        });
        seq.Append(tensionObj.transform.DORotate(new Vector3(0, 1080, 0), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutCirc));

        int index = 0;

        NodeActivate(seq, index);

        seq.AppendInterval(1f);
        seq.Append(gsnBar.rectTransform.DOScaleY(1, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendInterval(0.5f);
        seq.Append(gsnBar.rectTransform.DOScaleY(0, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendCallback(() =>
        {
            gsnArea.SetActive(false);
            StageManager.Instance.gameStatus = StageManager.GameState.Playing;
        });

        seq.Play();

        return seq;
    }

    private void NodeActivate(Sequence seq, int index)
    {
        seq.AppendCallback(() =>
        {
            railRoadList[index].SetActive(true);
            nodeList[index].SetActive(true);
            Instantiate(nodeActiveEffect, nodeList[index].transform.position, Quaternion.identity);
        });
        seq.AppendInterval(0.1f);
        if (index < nodeList.Count - 1)
        {
            NodeActivate(seq, index + 1);
        }
        else { return; }
    }

    /// <summary>
    /// 게임 오버 시의 연출 처리
    /// </summary>
    public void GameOver()
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            gsnArea.SetActive(true);
            gsnText.text = OVER_TEXT_VAL;
            gsnBar.color = new Color32(0, 0, 0, 200);
            StageManager.Instance.audioSource.Stop();
        });
        seq.Append(gsnBar.rectTransform.DOScaleY(1, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendInterval(1f);
        seq.Append(gsnPanel.rectTransform.DOScaleY(1, 0.5f));
        // 화면을 완전히 가린 후 저지 트레일러의 동작을 멈춘다.
        seq.AppendCallback(() =>
        {
            JudgeTrailerController.Instance.gameEnded = true;
        });

        seq.Play();
    }

    /// <summary>
    /// 게임 클리어 시의 연출 처리
    /// </summary>
    public void StageClear()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            JudgeTrailerController.Instance.gameEnded = true;
            gsnArea.SetActive(true);
            gsnText.text = CLEAR_TEXT_VAL;
            gsnBar.color = new Color32(0, 0, 0, 200);
            StageManager.Instance.audioSource.Stop();
        });
        seq.Append(gsnBar.rectTransform.DOScaleY(1, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendInterval(1f);
        seq.Append(gsnPanel.rectTransform.DOScaleY(1, 0.5f));
        // 화면을 완전히 가린 후 저지 트레일러의 동작을 멈춘다.
        seq.AppendCallback(() =>
        {
            StageManager.Instance.judgeInfoData.score = StageManager.Instance.Score;
            GameResultManager.Instance.ShowResultPanel(StageManager.Instance.judgeInfoData);
        });

        seq.Play();
    }
}
