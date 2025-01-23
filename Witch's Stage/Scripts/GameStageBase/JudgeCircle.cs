using TMPro;
using UnityEngine;

public class JudgeCircle : MonoBehaviour
{
    [Header("Circle Settings")]
    public float originRadius = 8f;
    public float targetRadius = 4f;
    public float targetFinaleRadius = 3f;
    public Material material;
    public GameObject particlePrefab;

    [HideInInspector] public float nowRadius = 8f;
    [HideInInspector] public int segments = 64;
    [HideInInspector] public float thickness = 0.1f;
    [HideInInspector] public float shrinkSpeed = 0.2f;
    [HideInInspector] public float showTime = 1f;
    [HideInInspector] public float shrinkTime = 1f;
    [HideInInspector] public float longTime = 1f;
    [HideInInspector] public float graceTime = 0.25f;
    [HideInInspector] public bool myTurn = false;
    [HideInInspector] public JudgeCircle nextCircle;

    protected bool playing = false;
    protected bool isFirstAct = true;
    protected float circleTime = 0;
    protected bool isJudged = false;

    void Awake()
    {
        originRadius *= 1 + (StageManager.Instance.StageData.speedMultiplier - 1) * 0.5f;
    }

    public void SetNextCircle(JudgeCircle nextCircle)
    {
        this.nextCircle = nextCircle;
    }

    public virtual void OnEnable() { playing = true; }
    public virtual void OnDisable() { playing = false; }
    public virtual void ActiveNextCircle()
    {
        myTurn = false;
        if (nextCircle != null)
        {
            nextCircle.myTurn = true;
            nextCircle.AppendInputEvent();
        }
    }

    public virtual void AppendInputEvent() { }

    public virtual void DoJudge(JudgeResult result)
    {
        if (result.Equals(JudgeResult.Miss))
        {
            ComboManager.instance.Miss();
        }
        else
        {
            ComboManager.instance.AddCombo();
        }

        switch (result)
        {
            case JudgeResult.Perfect:
                Debug.Log("Perfect");
                if (GameObject.Find("JudgeResult"))
                {
                    GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Perfect";
                }
                break;
            case JudgeResult.Good:
                Debug.Log("Good");
                if (GameObject.Find("JudgeResult"))
                {
                    GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Good";
                }
                break;
            case JudgeResult.Bad:
                Debug.Log("Bad");
                if (GameObject.Find("JudgeResult"))
                {
                    GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Bad";
                }
                break;
            case JudgeResult.Miss:
                Debug.Log("Miss");
                if (GameObject.Find("JudgeResult"))
                {
                    GameObject.Find("JudgeResult").GetComponent<TextMeshProUGUI>().text = $"{Mathf.Abs(circleTime - shrinkTime)} \n Miss";
                }
                break;
        }
        isJudged = true;
    }

    public void SetCircleTime(float shrinkTime, float graceTime, float longTime = 0f)
    {
        showTime = shrinkTime + graceTime;
        this.shrinkTime = shrinkTime;
        this.graceTime = graceTime;
        this.longTime = longTime;
    }

    public JudgeCircleController ToJugdeCircle()
    {
        return (JudgeCircleController)this;
    }

    public JudgeLongCircleController ToJudgeLongCircle()
    {
        return (JudgeLongCircleController)this;
    }
}