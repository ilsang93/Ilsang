using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DifficultySliderController : MonoBehaviour
{
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button extremeButton;
    [SerializeField] private GameObject difficultySelector;
    [SerializeField] private TextMeshProUGUI selectorText;

    public Difficulty Difficulty { get; private set; } = Difficulty.Extreme;

    private void Start()
    {
        FindObjectOfType<StageSelectInputManager>().OnLeftButtonPressed += InputLeft;
        FindObjectOfType<StageSelectInputManager>().OnRightButtonPressed += InputRight;

        easyButton.onClick.AddListener(() => ClickDifficultyButton(Difficulty.Easy));
        normalButton.onClick.AddListener(() => ClickDifficultyButton(Difficulty.Normal));
        hardButton.onClick.AddListener(() => ClickDifficultyButton(Difficulty.Hard));
        extremeButton.onClick.AddListener(() => ClickDifficultyButton(Difficulty.Extreme));
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        Difficulty = difficulty;
        FindObjectOfType<PanelRIghtController>().ChangeStageDifficulty((int)difficulty);
        Vector3 targetPos = GetButtonPosition(difficulty);
        difficultySelector.transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutBounce);
        selectorText.text = difficulty.ToString();
        PlayerPrefs.SetInt(StageSelectManager.Instance.selectedStageData.name + "_Difficulty", (int)difficulty);
    }

    private void ClickDifficultyButton(Difficulty difficulty)
    {
        if (difficulty == Difficulty) return;
        SetDifficulty(difficulty);
    }

    private Vector3 GetButtonPosition(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => easyButton.transform.localPosition,
            Difficulty.Normal => normalButton.transform.localPosition,
            Difficulty.Hard => hardButton.transform.localPosition,
            _ => extremeButton.transform.localPosition,
        };
    }

    private void InputLeft()
    {
        if (StageSelectManager.Instance.stageSelectState != StageSelectState.Normal) return;
        switch (Difficulty)
        {
            case Difficulty.Normal:
                if (StageSelectManager.Instance.selectedStageData.levelDataEasy)
                {
                    SetDifficulty(Difficulty.Easy);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Hard:
                if (StageSelectManager.Instance.selectedStageData.levelDataNormal)
                {
                    SetDifficulty(Difficulty.Normal);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataEasy)
                {
                    SetDifficulty(Difficulty.Easy);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Extreme:
                if (StageSelectManager.Instance.selectedStageData.levelDataHard)
                {
                    SetDifficulty(Difficulty.Hard);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataNormal)
                {
                    SetDifficulty(Difficulty.Normal);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataEasy)
                {
                    SetDifficulty(Difficulty.Easy);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Easy:
            default:
                break;
        }
    }

    private void InputRight()
    {
        if (StageSelectManager.Instance.stageSelectState != StageSelectState.Normal) return;
        switch (Difficulty)
        {
            case Difficulty.Easy:
                if (StageSelectManager.Instance.selectedStageData.levelDataNormal)
                {
                    SetDifficulty(Difficulty.Normal);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataHard)
                {
                    SetDifficulty(Difficulty.Hard);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataExtreme)
                {
                    SetDifficulty(Difficulty.Extreme);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Normal:
                if (StageSelectManager.Instance.selectedStageData.levelDataHard)
                {
                    SetDifficulty(Difficulty.Hard);
                    break;
                }
                else if (StageSelectManager.Instance.selectedStageData.levelDataExtreme)
                {
                    SetDifficulty(Difficulty.Extreme);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Hard:
                if (StageSelectManager.Instance.selectedStageData.levelDataExtreme)
                {
                    SetDifficulty(Difficulty.Extreme);
                    break;
                }
                else
                {
                    break;
                }
            case Difficulty.Extreme:
            default:
                break;
        }
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Extreme
}