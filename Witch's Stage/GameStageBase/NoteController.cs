using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject judgeRing;
    [SerializeField] private ParticleSystem judgeEffect;
    [HideInInspector] public bool myTurn = false;
    public NoteData noteData;
    private bool activated = false;
    private bool isJudged = false;

    void Start()
    {
        judgeEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isJudged) return;

        if (!activated)
        {
            // 테스트용 코드
            if (StageManager.Instance.playTime >= noteData.time - 0.5f)
            {
                StartCoroutine(StartJudge());
                activated = true;
            }
        }
    }

    // 판정 범위가 근처에 오면 판정을 시작한다.
    private IEnumerator StartJudge(float maxTime = 0.8f)
    {
        float time = 0;
        judgeRing.SetActive(true);
        judgeRing.transform.DOScale(new Vector3(1, 1, 1), 0.6f).SetEase(Ease.Linear);

        while (true)
        {
            time += Time.deltaTime;
            if (time >= maxTime)
            {
                judgeEffect.Play();
                break;
            }
            if (myTurn)
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
                {
                    float distance = Vector2.Distance(transform.position, JudgeTrailerController.Instance.transform.position);

                    if (distance < 0.5f)
                    {
                        Debug.Log("Good");
                    }
                    else
                    {
                        Debug.Log("Bad");

                    }
                    isJudged = true;
                    judgeRing.transform.DOKill();
                    StageManager.Instance.NextNodeTurn();
                    judgeEffect.Play();
                    break;
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(judgeEffect.main.duration);
        Destroy(gameObject);
    }
}