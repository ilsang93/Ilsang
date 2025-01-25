using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [SerializeField] private Button[] categoryButtons;

    private string testJson;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Button button in categoryButtons)
        {
            button.onClick.AddListener(() => OnCategoryButtonClick(button));
        }

        testJson = "";
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCategoryButtonClick(Button button)
    {

    }
}
