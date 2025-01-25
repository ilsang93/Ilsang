using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JudgementManager : MonoBehaviour
{
    public static JudgementManager instance;

    public GameObject judgeTargetPrefab;
    public Queue<GameObject> judgeTargetPool;
    public Queue<float> judgeTargetTime;
    private float nextTargetTime;

    private float tempTime; // 이 시간은 이후 게임 메니저가 관리하는 시간으로 변경한다.

    // Start is called before the first frame update
    void Start()
    {
        // judgeTargetTime 큐를 로드하고, 그 수만큼 타켓을 스택에 쌓는다.
        for (int i = 0; i < judgeTargetTime.Count; i++)
        {
            GameObject judgeTarget = Instantiate(judgeTargetPrefab);
            judgeTargetPool.Append(judgeTarget);
            judgeTarget.SetActive(false);
        }

        nextTargetTime = judgeTargetTime.Dequeue();
    }

    // 현재 FixedUpdateStep은 0.2로 설정한다.
    void FixedUpdate()
    {
        // queue에 저장된 시간에 도달하면, 게임 오브젝트를 활성화하고, 저장된 시간은 삭제한 후 다음 시간을 대기한다.
        if (tempTime >= nextTargetTime)
        {
            nextTargetTime = judgeTargetTime.Dequeue(); // 다음 타겟 시간을 대기하도록 한다.
            GameObject nowTarget = judgeTargetPool.Dequeue();
            nowTarget.SetActive(true); // 이 시점에서 다음 타겟을 활성화하고, OnEnable 내 처리가 실행되도록 한다.
        }
    }
}
