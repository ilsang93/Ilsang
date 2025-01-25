using UnityEngine;
using DG.Tweening;
using TMPro;

public class NotesManager : MonoBehaviour
{
    public static NotesManager instance;

    public GameObject rightPos;
    public GameObject leftPos;
    public GameObject centerPos;
    public GameObject notePrefab;
    public GameObject judgementObject;
    public float noteSpeed = 5f;
    public float noteTime = 0f;
    public float noteDuration = 1f;


    private TextMeshPro judgementText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        judgementText = judgementObject.GetComponentInChildren<TextMeshPro>();
        judgementText.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameState == GameState.Ready)
        {
            return;
        } else if (GameManager.instance.gameState != GameState.Play)
        {
            return;
        }
        
        noteTime += Time.deltaTime;
        if (noteTime > noteDuration)
        {
            CreateNote();
            noteTime = 0f;
        }
    }

    public void DoJudgement()
    {
        judgementText.transform.DOKill();
        judgementText.transform.localPosition = Vector3.zero;
        judgementText.color = Color.clear;

        judgementText.text = "Hit!";
        judgementText.DOColor(Color.green, 1).SetEase(Ease.Linear);
        judgementText.transform.DOLocalJump(new Vector3(0, 0, 0), 1, 1, 1).OnComplete(() => judgementText.text = "");

        //TODO 노트 이펙트 쉐이더를 실행해야함.

    }

    private void CreateNote()
    {
        // 좌우 포지션에 하나씩 생성한다
        GameObject rightNote = Instantiate(notePrefab, rightPos.transform.position, Quaternion.identity, transform);
        GameObject leftNote = Instantiate(notePrefab, leftPos.transform.position, Quaternion.identity, transform);

        // 좌우 노트의 위치를 사전 정의된 속도에 따라 중심으로 이동시킨다.
        rightNote.transform.DOLocalMoveX(0, 1 / noteSpeed).SetEase(Ease.Linear).OnComplete(() => Destroy(rightNote));
        leftNote.transform.DOLocalMoveX(0, 1 / noteSpeed).SetEase(Ease.Linear).OnComplete(() => Destroy(leftNote));
    }
}
