using System.Collections;
using TMPro;
using UnityEngine;

public class JudgeInfoController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI perfectCountText;
    [SerializeField] private TextMeshProUGUI goodCountText;
    [SerializeField] private TextMeshProUGUI badCountText;
    [SerializeField] private TextMeshProUGUI missCountText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [HideInInspector] public bool isAnimFinished = false;

    private int perfectCount;
    private int goodCount;
    private int badCount;
    private int missCount;
    private float score;

    void Awake()
    {
        perfectCountText.text = "0";
        goodCountText.text = "0";
        badCountText.text = "0";
        missCountText.text = "0";
        scoreText.text = "0";
    }

    public void SetInfo(JudgeInfoData data)
    {
        perfectCount = data.perfect;
        goodCount = data.good;
        badCount = data.bad;
        missCount = data.miss;
        score = data.score;
    }

    public IEnumerator DoAnim()
    {
        int loopCount = 0;
        // 판정 텍스트 업데이트트
        // 1/60초마다 실행
        while (true)
        {
            loopCount++;
            // 애니메이션 처리

            perfectCountText.text = $"{((perfectCount / 60 < 1 ? 1 : perfectCount / 60) * loopCount >= perfectCount ? perfectCount : (perfectCount / 60 < 1 ? 1 : perfectCount / 60) * loopCount)}";
            goodCountText.text = $"{((goodCount / 60 < 1 ? 1 : goodCount / 60) * loopCount >= goodCount ? goodCount : (goodCount / 60 < 1 ? 1 : goodCount / 60) * loopCount)}";
            badCountText.text = $"{((badCount / 60 < 1 ? 1 : badCount / 60) * loopCount >= badCount ? badCount : (badCount / 60 < 1 ? 1 : badCount / 60) * loopCount)}";
            missCountText.text = $"{((missCount / 60 < 1 ? 1 : missCount / 60) * loopCount >= missCount ? missCount : (missCount / 60 < 1 ? 1 : missCount / 60) * loopCount)}";

            yield return null;

            if (loopCount >= 59) break;
        }

        perfectCountText.text = $"{perfectCount}";
        goodCountText.text = $"{goodCount}";
        badCountText.text = $"{badCount}";
        missCountText.text = $"{missCount}";

        loopCount = 0;

        // 점수 텍스트 업데이트
        while (true)
        {
            loopCount++;
            // 애니메이션 처리
            scoreText.text = $"{score / 60 * loopCount}";
            yield return null;

            if (loopCount >= 59) break;
        }

        scoreText.text = $"{score}";

        isAnimFinished = true;

        yield break;
    }

    /// <summary>
    /// 판정 정보 패널 연출 애니메이션
    /// </summary>
    public void DoJudgeInfo()
    {
        isAnimFinished = true;
    }
}

public struct JudgeInfoData
{
    public int perfect;
    public int good;
    public int bad;
    public int miss;
    public float score;
}
