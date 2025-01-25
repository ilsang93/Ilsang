
using System.Collections.Generic;
using UnityEngine;

public struct TransferStageData
{
    public int diffIndex;
    public int level;
    public float speedMultiplier;
    public List<Vector2> nodeList;
    public List<NoteData> noteList;
    public AudioClip music;

    public TransferStageData(StageData stageData, Difficulty diffIndex = Difficulty.Easy, float speedMultiplyer = 1)
    {
        this.diffIndex = (int)diffIndex;
        level = stageData.levelData[(int)diffIndex].level;
        this.speedMultiplier = speedMultiplyer;
        nodeList = stageData.levelData[(int)diffIndex].nodeList;
        noteList = stageData.levelData[(int)diffIndex].noteList;
        music = stageData.stageMusic;
    }
}
