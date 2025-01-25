using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MusicElementController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI musicNameText;
    [SerializeField] private TextMeshProUGUI bpmText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image playSprite;
    [SerializeField] private Image diskImage;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite stopIcon;

    [HideInInspector] public MusicDatas musicDatas;

    private bool isPlaying = false;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = FindObjectOfType<MusicSelectDialogController>().musicDemoSource;

        playButton.onClick.AddListener(() =>
        {
            if (!isPlaying)
            {
                audioSource.Stop();

                isPlaying = true;
                playSprite.sprite = stopIcon;
                audioSource.clip = musicDatas.musicClip;
                audioSource.Play();

                diskImage.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
            }
            else
            {
                isPlaying = false;
                playSprite.sprite = playIcon;
                audioSource.clip = null;
                audioSource.Stop();
                diskImage.transform.DOKill();
                diskImage.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        });

        selectButton.onClick.AddListener(() =>
        {
            MusicSelectDialogController.Instance.nowMusicDatas = musicDatas;
            MusicSelectDialogController.Instance.transform.localScale = Vector3.zero;
            StageInspectorController.Instance.stageMusic.text = musicDatas.musicName;
        });
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (!audioSource.isPlaying)
            {
                isPlaying = false;
                playSprite.sprite = stopIcon;
                diskImage.transform.DOKill();
                diskImage.transform.rotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    public void SetMusicSO(AudioClip music)
    {
        // TODO MusicSO 불필요. 삭제 예정
        musicDatas = new(music.name, music);

        musicNameText.text = musicDatas.musicName;
        //FIXME BPM 굳이 필요한가?
        bpmText.text = "BPM : " + "TODO";
    }
}
