using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance;

    [SerializeField] private ComboController comboObj;
    [SerializeField] private MissController missObj;

    public int comboCount = 0;
    public int maxCombo = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        comboObj.gameObject.SetActive(false);
        missObj.gameObject.SetActive(false);
    }

    public void AddCombo()
    {
        comboCount++;

        if (comboCount > maxCombo)
        {
            maxCombo = comboCount;
        }

        comboObj.gameObject.SetActive(true);
        missObj.gameObject.SetActive(false);

        comboObj.AddCombo();
    }

    public void Miss()
    {
        comboCount = 0;

        comboObj.gameObject.SetActive(false);
        missObj.gameObject.SetActive(true);

        missObj.Miss();
    }
}
