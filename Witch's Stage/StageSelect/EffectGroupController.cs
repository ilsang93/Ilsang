using TMPro;
using UnityEngine;

public class EffectGroupController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;

    private const int MAX_SPEED = 5;
    [HideInInspector] public int nowSpeed = 1;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<StageSelectInputManager>().OnSpeedButtonPressed += OnPressSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnPressSpeed()
    {
        if (nowSpeed++ < MAX_SPEED)
        {
            speedText.text = nowSpeed.ToString();
        }
        else
        {
            nowSpeed = 1;
            speedText.text = 1.ToString();
        }
    }
}
