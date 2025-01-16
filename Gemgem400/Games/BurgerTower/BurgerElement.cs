using UnityEngine;

/// <summary>
/// 버거 재료 오브젝트를 제어하는 클래스
/// </summary>
public class BurgerElement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D thisRigid;

    public bool StopStatus { get; set; }
    public GameObject bugerObj;

    private bool soundPlay;
    private float stayTime;
    private bool timeCountFlg;
    private Vector3 offset;

    private void Start()
    {
        thisRigid = transform.GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if (bugerObj)
        {
            transform.position = bugerObj.transform.position + offset;
        }

        if (timeCountFlg)
        {
            stayTime += Time.deltaTime;
            if (stayTime > 2.5f && thisRigid.velocity.magnitude <= 0.2f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                thisRigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                timeCountFlg = false;
                StopStatus = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (soundPlay == false)
        {
            AudioManager.Instance.PlaySfx(Sfxs.CH_Landing);
            soundPlay = true;
        }
        timeCountFlg = true;
    }

    public void SetOffset(Vector3 BugetPos)
    {
        offset = transform.position - BugetPos;
    }
}