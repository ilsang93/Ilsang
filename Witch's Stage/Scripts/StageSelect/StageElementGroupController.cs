using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StageElementGroupController : MonoBehaviour
{
    [SerializeField] private float showElementCount;
    [SerializeField] private float elementRadius; // 반지름
    [SerializeField] private GameObject stageElementPrefab; // 스테이지 요소 프리팹
    [SerializeField] private float upperRadian, underRadian;
    [SerializeField] private float timeToHold;

    private List<Vector2> elementPositions = new();
    private List<StageElementController> stageElements = new();
    private StageElementController selectedElement;

    private float holdTime = 0;

    void Start()
    {
        CreateElementPositions();
        CreateStageElementGroup();

        stageElements[4].SetSelected(true);
        StageSelectManager.Instance.selectedStageData = stageElements[4].StageData;
        FindObjectOfType<PanelRIghtController>().SetStageData(PlayerPrefs.HasKey(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") ? (Difficulty)PlayerPrefs.GetInt(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") : Difficulty.Easy);

        FindObjectOfType<StageSelectInputManager>().OnUpButtonPressed += OnUpButtonPressed;
        FindObjectOfType<StageSelectInputManager>().OnDownButtonPressed += OnDownButtonPressed;
        FindObjectOfType<StageSelectInputManager>().OnUpButtonHold += HoldUpButton;
        FindObjectOfType<StageSelectInputManager>().OnDownButtonHold += HoldDownButton;
        FindObjectOfType<StageSelectInputManager>().OnReleaseUpDownButton += ReleaseUpDownButton;
    }

    private void CreateElementPositions()
    {
        // 원의 중심으로부터 우측에만 존재한다.
        for (int i = 0; i < showElementCount; i++)
        {
            float radian = Mathf.LerpAngle(upperRadian, underRadian, i / (showElementCount - 1)) * Mathf.Deg2Rad;

            float x = Mathf.Cos(radian) * elementRadius;
            float y = Mathf.Sin(radian) * elementRadius;

            elementPositions.Add(new Vector2(x, y));
        }
    }

    private void CreateStageElementGroup()
    {
        StageData[] stageDatas = Resources.LoadAll<StageData>("Datas/StageDatas");

        for (int i = 0; i < elementPositions.Count; i++)
        {
            StageElementController element = Instantiate(stageElementPrefab, transform).GetComponent<StageElementController>();
            // 임시 스테이지 데이터 삽입
            element.StageData = stageDatas[i % 2];
            element.SetHasLevel(element.StageData.levelDataEasy, element.StageData.levelDataNormal, element.StageData.levelDataHard, element.StageData.levelDataExtreme);
            stageElements.Add(element);
            element.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, elementPositions[i].y);
            DOTween.Sequence().
                AppendInterval(0.1f * i).
                Append(element.GetComponent<RectTransform>().DOAnchorPos(elementPositions[i], 0.1f).SetEase(Ease.OutBounce));
        }
    }

    private void HoldUpButton()
    {
        if (holdTime < timeToHold)
        {
            holdTime += Time.deltaTime;
        }
        else
        {
            OnUpButtonPressed();
            holdTime = 0;
        }
    }

    private void HoldDownButton()
    {
        if (holdTime < timeToHold)
        {
            holdTime += Time.deltaTime;
        }
        else
        {
            OnDownButtonPressed();
            holdTime = 0;
        }
    }

    private void ReleaseUpDownButton()
    {
        holdTime = 0;
    }

    private void OnUpButtonPressed()
    {
        StageElementController tempObj = stageElements[stageElements.Count - 1];
        // 위치 이동
        for (int i = stageElements.Count - 1; i >= 0; i--)
        {
            if (i == stageElements.Count - 1)
            {
                // 최하단 오브젝트는 최상단 위치로 즉시 이동한다.
                stageElements[i].GetComponent<RectTransform>().anchoredPosition = elementPositions[0];
                continue;
            }

            // 나머지 오브젝트는 자신의 바로 아래 오브젝트로 이동한다.
            stageElements[i].GetComponent<RectTransform>().DOAnchorPos(elementPositions[i + 1], 0.08f).SetEase(Ease.OutBounce);
        }

        // 리스트 인덱스 변경
        for (int i = stageElements.Count - 1; i >= 0; i--)
        {
            if (i == 0) continue;
            stageElements[i] = stageElements[i - 1];
        }

        stageElements[0] = tempObj;

        // 스테이지 선택 처리
        stageElements[5].SetSelected(false);
        stageElements[4].SetSelected(true);
        StageSelectManager.Instance.selectedStageData = stageElements[4].StageData;
        FindObjectOfType<PanelRIghtController>().SetStageData(PlayerPrefs.HasKey(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") ? (Difficulty)PlayerPrefs.GetInt(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") : Difficulty.Easy);
    }

    private void OnDownButtonPressed()
    {
        StageElementController tempObj = stageElements[0];

        for (int i = 0; i < stageElements.Count; i++)
        {
            if (i == 0)
            {
                // 최상단 오브젝트는 최하단 위치로 즉시 이동한다.
                stageElements[i].GetComponent<RectTransform>().anchoredPosition = elementPositions[^1];
                continue;
            }
            // 나머지 오브젝트는 자신의 바로 위 오브젝트 위치로 이동한다.
            stageElements[i].GetComponent<RectTransform>().DOAnchorPos(elementPositions[i - 1], 0.08f).SetEase(Ease.OutBounce);
        }

        // 리스트 인덱스 변경
        for (int i = 0; i < stageElements.Count; i++)
        {
            if (i == stageElements.Count - 1) continue;
            stageElements[i] = stageElements[i + 1];
        }

        stageElements[^1] = tempObj;

        // 스테이지 선택 처리
        stageElements[3].SetSelected(false);
        stageElements[4].SetSelected(true);
        StageSelectManager.Instance.selectedStageData = stageElements[4].StageData;
        FindObjectOfType<PanelRIghtController>().SetStageData(PlayerPrefs.HasKey(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") ? (Difficulty)PlayerPrefs.GetInt(StageSelectManager.Instance.selectedStageData.name + "_Difficulty") : Difficulty.Easy);
    }
}
