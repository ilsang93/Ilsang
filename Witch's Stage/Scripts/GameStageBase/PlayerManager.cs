using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public CharacterManager characterManager;
    public Vector3 PlayerPosition
    {
        get
        {
            return characterManager.transform.position;
        }
        set
        {
            characterManager.transform.position = value;
        }
    }
    [HideInInspector]
    public ChracterStatus chracterStatus = ChracterStatus.Idle;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum ChracterStatus
    {
        Idle,
        Move,
        Attack,
        Dead
    }
}
