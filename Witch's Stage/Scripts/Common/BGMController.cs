using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static BGMController instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBGM(string bgmName)
    {
        audioSource.clip = Resources.Load<AudioClip>("Sounds/BGM/" + bgmName);
        audioSource.Play();
    }

    public void StopBGM()
    {
        if (audioSource) audioSource.Stop();
    }
}
