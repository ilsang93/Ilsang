using UnityEngine;

public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    public static SoundController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetMusic(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public void StartMusic()
    {
        audioSource.Play();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    // 효과음을 재생한다.
    public void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
