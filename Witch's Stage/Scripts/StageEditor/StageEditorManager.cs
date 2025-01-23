using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class StageEditorManager : MonoBehaviour
{
    public static StageEditorManager Instance { get; private set; }

    // Action
    public Action OnStageDataLoaded;

    // SerilaizeField
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject filedNotePrefab;
    [SerializeField] private GameObject timeLineNotePrefab;

    [SerializeField] private GameObject firstNode;

    [SerializeField] private GameObject nodeLayer;
    [SerializeField] private GameObject railRoadLayer;
    [SerializeField] private GameObject tlNoteLayer;
    [SerializeField] private GameObject flNoteLayer;

    [SerializeField] private TMP_Dropdown editorMode;
    [SerializeField] private TextMeshProUGUI musicNameText;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

    // Buttons
    [SerializeField] private Button addNoteButton;
    [SerializeField] private Button removeNoteButton;
    [SerializeField] private Button editSettingsButton;

    [SerializeField] private GameObject addNodeModeGuide;
    [SerializeField] private GameObject editNoteModeGuide;

    // Virtual Camera
    [SerializeField] private CinemachineCamera editorCamera;
    [SerializeField] private CinemachineCamera demoPlayCamera;


    // HideInInspector
    [HideInInspector] public List<GameObject> railRoadList = new();
    [HideInInspector] public EditorStatus editorStatus = EditorStatus.Idle;
    [HideInInspector] public List<GameObject> noteList = new();
    [HideInInspector] public Dictionary<GameObject, GameObject> notePairDic = new();
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public AudioClip stageMusic;
    [HideInInspector] public bool isOtherProcess = false;
    [HideInInspector] public List<GameObject> nodeList = new();
    [HideInInspector] public StageData stageData;
    // public

    // Private
    private GameObject lastNode, nowNode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 카메라 초기화
        CameraChange(true);

        audioSource = GetComponent<AudioSource>();

        nodeList.Add(firstNode);
        lastNode = firstNode;

        editorMode.onValueChanged.AddListener((int value) =>
        {
            OnModeChanged(value);
        });

        addNoteButton.onClick.AddListener(() =>
        {
            // 선택한 타임라인 secondLine에 노트 추가
            AddNote();
        });

        removeNoteButton.onClick.AddListener(() =>
        {
            RemoveNote();
        });

        editSettingsButton.onClick.AddListener(() =>
        {
            // 스테이지 정보 수정
            OpenEditSettingsDialog();
            isOtherProcess = true;
        });

        FindObjectOfType<LoadButtonController>().OnLoadButtonClicked += OpenLoadDataDialog;
    }

    void Update()
    {
        if (editorStatus == EditorStatus.AddNode && nowNode != null)
        {
            //MEMO Vector2로 형변환 하지 않으면 direction에 거리가 반영되는 문제가 생김.
            Vector2 direction = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)lastNode.transform.position).normalized;
            nowNode.transform.position = direction * StageConstant.NODE_DISTANCE + (Vector2)lastNode.transform.position;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddNode();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                RemoveNode();
            }
        }
    }

    public void OnModeChanged(int value)
    {
        if (editorStatus == (EditorStatus)value) return;

        // 모드를 변경하지 전에 이전 모드에서의 작업을 정리한다.
        switch (editorStatus)
        {
            case EditorStatus.Idle:
                break;
            case EditorStatus.AddNode:
                addNodeModeGuide.SetActive(false);
                Destroy(nowNode);
                break;
            case EditorStatus.EditNote:
                editNoteModeGuide.SetActive(false);
                EditorRailController[] railControllers = FindObjectsOfType<EditorRailController>();
                foreach (EditorRailController erc in railControllers)
                {
                    erc.SetSelected(false);
                }
                SecondLineController[] slControllers = FindObjectsOfType<SecondLineController>();
                foreach (SecondLineController slc in slControllers)
                {
                    slc.SetSelected(false);
                }
                EditorNoteController[] editorNoteController = FindObjectsOfType<EditorNoteController>();
                foreach (EditorNoteController noteController in editorNoteController)
                {
                    noteController.SetSelected(false);
                }
                break;
            case EditorStatus.DemoPlay:
                break;
        }

        editorStatus = (EditorStatus)value;

        // 모드에 따라 초기화 작업을 수행한다.
        switch (editorStatus)
        {
            case EditorStatus.Idle:
                break;
            case EditorStatus.AddNode:
                addNodeModeGuide.SetActive(true);
                nowNode = Instantiate(nodePrefab, firstNode.transform.position + new Vector3(1, 0, 0), Quaternion.identity, nodeLayer.transform);
                break;
            case EditorStatus.EditNote:
                editNoteModeGuide.SetActive(true);
                break;
            case EditorStatus.DemoPlay:
                break;
        }
    }

    public void AddNode()
    {
        nodeList.Add(nowNode);

        GameObject railRoad = Instantiate(linePrefab, railRoadLayer.transform);
        railRoad.transform.position = (nowNode.transform.position + lastNode.transform.position) / 2;
        railRoad.transform.up = nowNode.transform.position - lastNode.transform.position;
        railRoad.GetComponent<SpriteRenderer>().size = new Vector2(StageConstant.RAIL_WIDTH, StageConstant.NODE_DISTANCE);
        railRoad.GetComponent<EditorRailController>().SetLineIndex(nodeList.Count - 2);
        railRoadList.Add(railRoad);

        lastNode = nowNode;
        nowNode = Instantiate(nodePrefab, lastNode.transform.position + new Vector3(1, 0, 0), Quaternion.identity, nodeLayer.transform);
        editorCamera.transform.DOKill();
        editorCamera.transform.DOMove(new Vector3(nowNode.transform.position.x, nowNode.transform.position.y, Camera.main.transform.position.z), 0.5f).SetEase(Ease.InSine);

        NoteTimeLineController.Instance.Add2SecondLine();
    }

    public void RemoveNode()
    {
        if (nodeList.Count <= 1) return;

        Destroy(nowNode);

        Destroy(lastNode);
        lastNode = nodeList[^2];
        nodeList.RemoveAt(nodeList.Count - 1);

        Destroy(railRoadList[^1]);
        railRoadList.RemoveAt(railRoadList.Count - 1);

        nowNode = Instantiate(nodePrefab, lastNode.transform.position + new Vector3(1, 0, 0), Quaternion.identity);
        editorCamera.transform.DOKill();
        editorCamera.transform.DOMove(new Vector3(nowNode.transform.position.x, nowNode.transform.position.y, Camera.main.transform.position.z), 0.5f).SetEase(Ease.InSine);

        EditorNoteController[] notes = FindObjectsOfType<EditorNoteController>();

        foreach (EditorNoteController note in notes)
        {
            if (note.targetTime > (nodeList.Count - 1) * 2)
            {
                noteList.Remove(note.gameObject);
                notePairDic.Remove(note.pairNote);
                Destroy(note.gameObject);
                Destroy(note.pairNote);
            }
        }
        NoteTimeLineController.Instance.Remove2SecondLine();
    }

    public void SelectRailRoad(int index)
    {
        SecondLineController[] slControllers = FindObjectsOfType<SecondLineController>();
        foreach (SecondLineController slc in slControllers)
        {
            slc.SetSelected(false);
        }
        EditorNoteController[] editorNoteController = FindObjectsOfType<EditorNoteController>();
        foreach (EditorNoteController noteController in editorNoteController)
        {
            noteController.SetSelected(false);
        }
        foreach (GameObject railRoad in railRoadList)
        {
            railRoad.GetComponent<EditorRailController>().SetSelected(false);
        }
        railRoadList[index].GetComponent<EditorRailController>().SetSelected(true);
    }

    public void DeselectAllRailRoad()
    {
        foreach (GameObject railRoad in railRoadList)
        {
            railRoad.GetComponent<EditorRailController>().SetSelected(false);
        }
    }

    public void SelectSecondLine(SecondLineController secondLineController)
    {
        SecondLineController[] slControllers = FindObjectsOfType<SecondLineController>();
        foreach (SecondLineController slc in slControllers)
        {
            slc.SetSelected(false);
        }
        EditorNoteController[] editorNoteController = FindObjectsOfType<EditorNoteController>();
        foreach (EditorNoteController noteController in editorNoteController)
        {
            noteController.SetSelected(false);
        }

        NoteTimeLineController.Instance.MoveToSelectedLine((int)secondLineController.Time / 2);
        secondLineController.SetSelected(true);
    }

    public void AddNote()
    {
        bool hasSelected = false;
        SecondLineController targetSlc = null;
        float time;

        SecondLineController[] slControllers = FindObjectsOfType<SecondLineController>();
        foreach (SecondLineController slc in slControllers)
        {
            if (slc.isSelected)
            {
                if (slc.ThisTlNote)
                {
                    CommonMessageManager.Instance.ShowMessage("이미 노트가 존재합니다.");
                    break;
                }
                targetSlc = slc;
                hasSelected = true;
                break;
            }
        }

        if (!hasSelected || targetSlc == null)
        {
            CommonMessageManager.Instance.ShowMessage("선택된 시간이 없습니다.");
            return;
        }

        GameObject tlnote = Instantiate(timeLineNotePrefab, new Vector3(), Quaternion.identity, tlNoteLayer.transform);
        GameObject fnote = Instantiate(filedNotePrefab, new Vector3(), Quaternion.identity, flNoteLayer.transform);

        time = targetSlc.Time;

        tlnote.transform.localPosition =
            new Vector3(targetSlc.transform.parent.transform.localPosition.x + targetSlc.transform.localPosition.x + targetSlc.GetComponent<RectTransform>().sizeDelta.x / 2,
                -50f, targetSlc.transform.localPosition.z);

        // 필드 노트 위치 설정
        int index = (int)time / 2 + 1;
        // 양 끝의 노드를 취득하고, 그 사이에서 시간에 해당하는 노트의 위치를 구한다.
        fnote.transform.position = Vector2.Lerp(nodeList[index - 1].transform.position, nodeList[index].transform.position, time % 2 / 2);

        tlnote.GetComponent<EditorNoteController>().targetTime = time;
        fnote.GetComponent<EditorNoteController>().targetTime = time;

        targetSlc.ThisTlNote = tlnote;
        targetSlc.ThisFlNote = fnote;
        noteList.Add(tlnote);
        notePairDic.Add(tlnote, fnote);
    }

    public void RemoveNote()
    {
        SecondLineController[] slControllers = FindObjectsOfType<SecondLineController>();
        foreach (SecondLineController slc in slControllers)
        {
            if (slc.isSelected)
            {
                if (slc.ThisTlNote != null)
                {
                    GameObject target = slc.ThisTlNote;
                    GameObject targetPair = notePairDic[slc.ThisTlNote];

                    noteList.Remove(target);
                    notePairDic.Remove(targetPair);
                    Destroy(target);
                    Destroy(targetPair);
                }
                break;
            }
        }
    }

    public void DataToEditor(StageData stageData)
    {
        nodeList.ForEach(Destroy);
        noteList.ForEach(Destroy);
        railRoadList.ForEach(Destroy);

        nodeList.Clear();
        noteList.Clear();
        railRoadList.Clear();

        // 최초 노드 생성 : 최초 노드는 항상 0,0에 위치한다.
        firstNode = Instantiate(nodePrefab, Vector2.zero, Quaternion.identity, nodeLayer.transform);
        nodeList.Add(firstNode);

        // 노드 생성
        for (int i = 1; i < stageData.levelData[0].jsonNodeList.Count; i++)
        {
            // 노드 생성 처리
            GameObject node = Instantiate(nodePrefab, stageData.levelData[0].jsonNodeList[i].ToVector2(), Quaternion.identity, nodeLayer.transform);
            nodeList.Add(node);

            // 레일로드 생성 처리
            GameObject railRoad = Instantiate(linePrefab, railRoadLayer.transform);
            railRoad.transform.position = (node.transform.position + nodeList[i - 1].transform.position) / 2;
            railRoad.transform.up = node.transform.position - nodeList[i - 1].transform.position;
            railRoad.GetComponent<SpriteRenderer>().size = new Vector2(StageConstant.RAIL_WIDTH, StageConstant.NODE_DISTANCE);
            railRoad.GetComponent<EditorRailController>().SetLineIndex(i - 1);
            railRoadList.Add(railRoad);

            // 타임라인 생성 처리
            NoteTimeLineController.Instance.Add2SecondLine();
        }
        lastNode = nodeList[^1];

        // 노트 생성
        for (int i = 0; i < stageData.levelData[0].noteList.Count; i++)
        {
            GameObject tlnote = Instantiate(timeLineNotePrefab, new Vector3(), Quaternion.identity, tlNoteLayer.transform);
            GameObject fnote = Instantiate(filedNotePrefab, new Vector3(), Quaternion.identity, flNoteLayer.transform);

            EditorNoteController tlnoteController = tlnote.GetComponent<EditorNoteController>();
            EditorNoteController fnoteController = fnote.GetComponent<EditorNoteController>();

            tlnoteController.targetTime = stageData.levelData[0].noteList[i].time;
            fnoteController.targetTime = stageData.levelData[0].noteList[i].time;

            tlnoteController.pairNote = fnote;
            fnoteController.pairNote = tlnote;

            tlnoteController.isTimeLineNote = true;
            fnoteController.isTimeLineNote = false;

            // 타임라인 노트 생성 처리
            SecondLineController[] lines = FindObjectsOfType<SecondLineController>();
            foreach (SecondLineController line in lines)
            {
                if (line.Time == tlnoteController.targetTime)
                {
                    line.ThisTlNote = tlnote;
                    tlnote.transform.localPosition = new Vector3(line.transform.parent.transform.localPosition.x + line.transform.localPosition.x + line.GetComponent<RectTransform>().sizeDelta.x / 2, -50f, line.transform.localPosition.z);
                    break;
                }
            }

            // 필드 노트 생성 처리
            //TODO 선택 레벨에 따른 추가 처리
            int index = (int)stageData.levelData[0].noteList[i].time / 2 + 1;
            fnote.transform.position = Vector2.Lerp(nodeList[(int)stageData.levelData[0].noteList[i].time / 2].transform.position, nodeList[(int)stageData.levelData[0].noteList[i].time / 2 + 1].transform.position, stageData.levelData[0].noteList[i].time % 2 / 2);

            noteList.Add(tlnote);
            notePairDic.Add(tlnote, fnote);
        }

        this.stageData = stageData;
        
        MusicSelectDialogController.Instance.nowMusicDatas = new MusicDatas(stageData.musicName, stageData.stageMusic);
        MusicSelectDialogController.Instance.beforeMusicDatas = MusicSelectDialogController.Instance.nowMusicDatas;

        BackgroundSelectController.Instance.nowSprite = stageData.backgroundSprite;
        BackgroundSelectController.Instance.beforeSprite = BackgroundSelectController.Instance.nowSprite;

        SetMusic(new MusicDatas(stageData.musicName, stageData.stageMusic));
        SetBackground(stageData.backgroundSprite);

        OnStageDataLoaded?.Invoke();
    }

    private void OpenLoadDataDialog()
    {
        DataLoadDialogManager.Instance.transform.localScale = Vector3.one;
    }

    private void OpenEditSettingsDialog()
    {
        StageInspectorController.Instance.transform.localScale = Vector3.one;
    }

    public void SetMusic(MusicDatas music)
    {
        stageMusic = music.musicClip;
        stageMusic.name = music.musicName;
        audioSource.clip = stageMusic;
        musicNameText.text = music.musicName;
        isOtherProcess = false;
    }

    public void SetBackground(Sprite sprite)
    {
        backgroundSpriteRenderer.sprite = sprite;
        // 사이즈는 aprite의 크기에 맞춰서 조정한다.
        backgroundSpriteRenderer.size = new Vector2(sprite.rect.width, sprite.rect.height) * 100f;

        // 카메라의 뷰포트 높이와 너비 계산
        float screenHeight = 2f * demoPlayCamera.Lens.OrthographicSize;
        float screenWidth = screenHeight * demoPlayCamera.Lens.Aspect;

        // 스프라이트의 크기 가져오기
        float spriteWidth = sprite.bounds.size.x;
        float spriteHeight = sprite.bounds.size.y;

        // 스프라이트의 비율을 유지하면서 화면에 맞추기 위한 스케일 계산
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;
        float scale = Mathf.Max(scaleX * 2, scaleY * 2); // 비율을 유지하기 위해 더 큰 스케일 적용

        // 스프라이트의 스케일 조정
        backgroundSpriteRenderer.transform.localScale = new Vector3(scale, scale, 1);
    }

    public void CameraChange(bool mainFlg)
    {
        if (mainFlg)
        {
            editorCamera.Priority = 1;
            demoPlayCamera.Priority = 0;
        }
        else
        {
            editorCamera.Priority = 0;
            demoPlayCamera.Priority = 1;
        }
    }
}

public enum EditorStatus
{
    Idle,
    AddNode,
    EditNote,
    DemoPlay,
}