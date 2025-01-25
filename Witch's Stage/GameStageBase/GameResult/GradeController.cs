using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GradeController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [SerializeField] private GameObject gradeObj;
    [SerializeField] private GameObject plusObj;

    private const int GRADE_S_PLUS = 950000;
    private const int GRADE_S = 900000;
    private const int GRADE_A_PLus = 850000;
    private const int GRADE_A = 800000;
    private const int GRADE_B_PLUS = 750000;
    private const int GRADE_B = 700000;
    private const int GRADE_C_PLUS = 650000;
    private const int GRADE_C = 600000;

    void Start()
    {
        // 초기화
        gradeObj.SetActive(true);
        gradeObj.transform.localScale = Vector3.one * 2;
        gradeObj.transform.rotation = Quaternion.Euler(0, 0, 90);
        gradeText.alpha = 0;
        plusObj.GetComponent<TextMeshProUGUI>().alpha = 0;
    }

    public void SetGrade(float score)
    {
        if (score >= GRADE_S_PLUS)
        {
            gradeText.text = "S";
            plusObj.SetActive(true);
        }
        else if (score >= GRADE_S)
        {
            gradeText.text = "S";
            plusObj.SetActive(false);
        }
        else if (score >= GRADE_A_PLus)
        {
            gradeText.text = "A";
            plusObj.SetActive(true);
        }
        else if (score >= GRADE_A)
        {
            gradeText.text = "A";
            plusObj.SetActive(false);
        }
        else if (score >= GRADE_B_PLUS)
        {
            gradeText.text = "B";
            plusObj.SetActive(true);
        }
        else if (score >= GRADE_B)
        {
            gradeText.text = "B";
            plusObj.SetActive(false);
        }
        else if (score >= GRADE_C_PLUS)
        {
            gradeText.text = "C";
            plusObj.SetActive(true);
        }
        else if (score >= GRADE_C)
        {
            gradeText.text = "C";
            plusObj.SetActive(false);
        }
        else
        {
            gradeText.text = "D";
            plusObj.SetActive(false);
        }

        maxComboText.text = $"{ComboManager.instance.maxCombo}";
    }

    public IEnumerator DoAnim()
    {
        gradeText.DOFade(1, 0.5f);
        plusObj.GetComponent<TextMeshProUGUI>().DOFade(1, 0.5f);
        gradeObj.transform.DOScale(Vector3.one, 0.5f);
        gradeObj.transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f);

        yield break;
    }
}
