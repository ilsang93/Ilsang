using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "Stage")]
public class StageSO : ScriptableObject
{
    public string stageId;
    public AudioSource bgmSource;
}
