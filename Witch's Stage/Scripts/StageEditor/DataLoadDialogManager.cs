using UnityEngine;
using UnityEngine.UI;

public class DataLoadDialogManager : MonoBehaviour
{
    public static DataLoadDialogManager Instance { get; private set; }
    [SerializeField] private Transform loadableDataContainer;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject laycastMask;
    [SerializeField] private GameObject elementPrefab;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            transform.localScale = Vector3.zero;
        });
    }

    public void AddData(StageData stageData)
    {
        GameObject element = Instantiate(elementPrefab, loadableDataContainer);
        element.GetComponent<StageLoadElementController>().SetStageData(stageData);
    }

    public void ResetAllData()
    {
        foreach (Transform child in loadableDataContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void LoadData(StageData stageData)
    {
        StageEditorManager.Instance.DataToEditor(stageData);
        transform.localScale = Vector3.zero;
    }
}
