using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1초의 길이는 4임.
/// 배속시 초의 길이가 길어지는 만큼, 레일의 길이도 길어져야 함.
/// </summary>
public class StageGenerator : MonoBehaviour
{
    [SerializeField] private GameObject railRoadLayer;
    [SerializeField] private GameObject railRoadPrefab;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject noteLayer;

    [HideInInspector] public List<GameObject> railRoadList;
    [HideInInspector] public List<GameObject> nodeList;

    private Vector2 lastNodePosition;

    private void Start()
    {
        StageManager.Instance.audioSource.Play();
        StageManager.Instance.audioSource.Pause();
        GenerateStage();
    }

    private void GenerateStage()
    {
        // 노드 포지션 취득 필요
        StageManager.Instance.railNodes = StageManager.Instance.StageData.nodeList.ConvertAll(node => node * StageManager.Instance.StageData.speedMultiplier);

        GenerateRailroad(StageManager.Instance.railNodes, StageManager.Instance.StageData.noteList, StageManager.Instance.StageData.speedMultiplier);

        FindObjectOfType<GameStatusNoticeManager>().InitOnStart(railRoadList, nodeList);
    }

    private NoteData SetNotePositionByTime(NoteData note)
    {
        // 시작 4초 후 부터 노트를 생성하는 임시 처리
        float distanceFromStart = StageManager.Instance.Speed * note.time;

        float distance = 0;
        for (int i = 1; i < StageManager.Instance.railNodes.Count; i++)
        {
            float tempDistance = distance + Vector2.Distance(StageManager.Instance.railNodes[i], StageManager.Instance.railNodes[i - 1]);
            if (tempDistance >= distanceFromStart)
            {
                // 방향을 구한다.
                Vector2 direction = (StageManager.Instance.railNodes[i] - StageManager.Instance.railNodes[i - 1]).normalized;
                // i - 1 위치에서 distanceFromStart만큼 떨어진 위치를 구한다.
                Vector2 resultPosition = StageManager.Instance.railNodes[i - 1] + (direction * (distanceFromStart - distance));
                // 노트의 위치를 설정한다.
                note.position = resultPosition;
                return note;
            }
            else
            {
                distance = tempDistance;
            }
        }
        throw new System.Exception("노트 위치 설정 실패");
    }

    private void GenerateRailroad(List<Vector2> nodes, List<NoteData> noteList, float speedMultiplyer = 1)
    {
        railRoadList = new();
        nodeList = new();

        for (int i = 1; i < nodes.Count; i++)
        {
            // 노드 생성 처리
            GameObject node = Instantiate(nodePrefab, nodes[i], Quaternion.identity, railRoadLayer.transform);
            nodeList.Add(node);

            // 레일로드 생성 처리
            LineMeshController railRoad = Instantiate(railRoadPrefab, railRoadLayer.transform).GetComponent<LineMeshController>();
            railRoad.GenerateLine(nodes[i - 1], nodes[i]);
            railRoadList.Add(railRoad.gameObject);
        }

        List<NoteData> notes = new();

        int scoreUnitCount = 0;

        for (int i = 1; i < noteList.Count; i++)
        {
            notes.Add(SetNotePositionByTime(noteList[i]));

            if (noteList[i].isLong)
            {
                scoreUnitCount += 2;
            }
            else
            {
                scoreUnitCount++;
            }
        }

        StageManager.Instance.ScoreUnit = 1000000 / scoreUnitCount / 3;

        foreach (var note in notes)
        {
            NoteController noteObject = Instantiate(notePrefab, note.position, Quaternion.identity, noteLayer.transform).GetComponent<NoteController>();
            StageManager.Instance.noteList.Add(noteObject);
            noteObject.noteData = note;
        }
    }
}