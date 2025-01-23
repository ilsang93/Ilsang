using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private SFXController sfxController;
    private BGMController bgmController;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sfxController = SFXController.instance;
        bgmController = BGMController.instance;
    }

    public void PlaySFX(string sfxName)
    {
        sfxController.PlaySFX(sfxName);
    }

    public void PlayBGM(string bgmName)
    {
        bgmController.PlayBGM(bgmName);
    }

    public void StopBGM()
    {
        bgmController.StopBGM();
    }
}
