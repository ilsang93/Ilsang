using System.Collections.Generic;
using UnityEngine;

public class JudgeCircleManager : MonoBehaviour
{
    [SerializeField] private JudgeCircleController judgeCirclePrefab;
    [SerializeField] private JudgeLongCircleController judgeLongCirclePrefab;

    private Queue<float> timeQueue;
    private Queue<JudgeCircle> circleQueue;

    private float fixedTime = 0;

    // 1 / 60초 간격으로 처리를 진행한다.
    void FixedUpdate()
    {
        if (StageManager.Instance.gameStatus != StageManager.GameState.Playing) return;

        fixedTime += Time.fixedDeltaTime;

        if (timeQueue.Count > 0 && fixedTime >= timeQueue.Peek())
        {
            JudgeCircle tempCircle = circleQueue.Dequeue();
            tempCircle.gameObject.SetActive(true);
            timeQueue.Dequeue();
            return;
        }
    }

    public void CreateCirclePool(List<NoteData> noteList)
    {
        timeQueue = new();
        circleQueue = new();

        JudgeCircle lastCircle = null;

        // 원이 축소되는 시간과 판정 타겟 타임을 지정한다.
        float prefixTime = 1f;
        // 원이 완전히 축소된 후, 유예 시간을 지정한다.
        float graceTime = 0.2f;

        for (int i = 0; i < noteList.Count; i++)
        {
            JudgeCircle tempCircle;
            if (noteList[i].isLong)
            {
                tempCircle = Instantiate(judgeLongCirclePrefab, transform).GetComponent<JudgeLongCircleController>();
                tempCircle.gameObject.name = $"Circle_{i}_Long";
                tempCircle.SetCircleTime(prefixTime, graceTime, noteList[i].longTime);
            }
            else
            {
                tempCircle = Instantiate(judgeCirclePrefab, transform).GetComponent<JudgeCircleController>();
                tempCircle.gameObject.name = $"Circle_{i}";
                tempCircle.SetCircleTime(prefixTime, graceTime);
            }

            //TODO 조건에 따라 판정원의 두께를 조절해야 함.
            tempCircle.GetComponent<JudgeCircle>().thickness = 0.1f;
            circleQueue.Enqueue(tempCircle);

            // 원이 축소되는 시간을 반영하여, 원을 활성화 할 시간을 큐에 저장한다.
            timeQueue.Enqueue(noteList[i].time - prefixTime);
            tempCircle.gameObject.SetActive(false);

            if (i == 0)
            {
                tempCircle.myTurn = true;
                tempCircle.AppendInputEvent();
                lastCircle = tempCircle;
            }
            else
            {
                lastCircle.SetNextCircle(tempCircle);
                lastCircle = tempCircle;
            }
        }
    }
}
