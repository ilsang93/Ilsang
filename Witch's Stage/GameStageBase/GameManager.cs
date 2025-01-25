using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState gameState = GameState.Ready;
    public ReadyController readyController;

    [SerializeField]
    private AudioSource bgmSource;
    public AudioSource BGMSource { get; }
    [SerializeField]
    private AudioSource sfxSource;
    public AudioSource SFXSource { get; }

    void Awake()
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
    // Start is called before the first frame update
    void Start()
    {
        readyController.DoReady();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public enum GameState
{
    Ready,
    Play,
    Pause,
    GameOver
}
