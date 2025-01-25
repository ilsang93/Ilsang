using UnityEngine;

public class LongnoteDevManager : MonoBehaviour
{
    [SerializeField] private GameObject longnotePrefab;
    [SerializeField] private Vector3 noteProp;

    private GameObject nowNote;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!nowNote || nowNote.activeSelf == false)
        {
            nowNote = Instantiate(longnotePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            nowNote.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            nowNote.GetComponent<JudgeLongCircleController>().SetCircleTime(noteProp.x, noteProp.y, noteProp.z);
        }
    }
}
