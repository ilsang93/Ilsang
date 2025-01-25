
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int diffIndex; // 0: EASY, 1: NORMAL, 2: HARD, 3: EXTREME
    public int level; // 1 ~ 20
    public List<JsonVector2> jsonNodeList;
    [JsonIgnore] public List<Vector2> nodeList;
    public List<NoteData> noteList;
}

[System.Serializable]
public struct NoteData
{
    [JsonIgnore] public Vector2 position;
    public float time;
    public bool isLong;
    public float longTime;
}