using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{
    [SerializeField] private Button workshopButton;
    [SerializeField] private Button editorButton;
    // Start is called before the first frame update
    void Start()
    {
        workshopButton.onClick.AddListener(OnClickWorkshopButton);
        editorButton.onClick.AddListener(OnClickEditorButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClickEditorButton()
    {
        StartCoroutine(SceneLoadManager.Instance.LoadScene("StageEditor"));
    }

    private void OnClickWorkshopButton()
    {
        //TODO Workshop Steam Page Load
    }
}
