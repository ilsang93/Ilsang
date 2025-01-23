using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelIconController : MonoBehaviour
{
    [SerializeField] private Image[] icons;

    [SerializeField] private Color starFrom1;
    [SerializeField] private Color starFrom4;
    [SerializeField] private Color starFrom9;
    [SerializeField] private Color starFrom18;

    private Vector3 rotateVal = new Vector3(0, 360, 0);

    void Start()
    {
        SetColors();
        SetLevel(0);
    }

    public void SetLevel(int level)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < level; i++)
        {
            icons[i].gameObject.SetActive(true);
        }
    }

    private void SetColors()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i < 3)
            {
                icons[i].color = starFrom1;
            }
            else if (i < 8)
            {
                icons[i].color = starFrom4;
            }
            else if (i < 17)
            {
                icons[i].color = starFrom9;
            }
            else
            {
                icons[i].color = starFrom18;
            }
        }
    }
}
