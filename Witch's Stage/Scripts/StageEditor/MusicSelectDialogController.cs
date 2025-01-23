using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectDialogController : MonoBehaviour
{
    public static MusicSelectDialogController Instance { get; private set; }
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject musicElementPrefab;
    [SerializeField] private Transform musicListContainer;
    
    [HideInInspector] public AudioSource musicDemoSource;
    [HideInInspector] public MusicDatas nowMusicDatas;
    [HideInInspector] public MusicDatas beforeMusicDatas;

    private const string STAGE_MUSIC_PATH = "/StageMusic";
    private string stageMusicPath;

    private void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        stageMusicPath = Application.persistentDataPath + STAGE_MUSIC_PATH;
        musicDemoSource = GetComponent<AudioSource>();

        Initialize();

        closeButton.onClick.AddListener(() =>
        {
            musicDemoSource.Stop();
            transform.localScale = Vector3.zero;
        });
    }

    private void Initialize()
    {
        List<AudioClip> musics = LoadMusicDatas();

        foreach (AudioClip music in musics)
        {
            GameObject musicElement = Instantiate(musicElementPrefab, musicListContainer);
            musicElement.GetComponent<MusicElementController>().SetMusicSO(music);
        }
    }

    public List<AudioClip> LoadMusicDatas()
    {
        List<AudioClip> musicDatas = new();

        DirectoryInfo di = new(stageMusicPath);
        if (!di.Exists) di.Create();
        FileInfo[] files = di.GetFiles("*.wav");

        foreach (FileInfo file in files)
        {
            byte[] wavFileBytes = File.ReadAllBytes(file.FullName);
            //TODO mp3 파일을 읽어서 AudioClip으로 변환
            if (wavFileBytes == null || wavFileBytes.Length == 0)
            {
                Debug.LogError("WAV file is empty or not found.");
            }

            musicDatas.Add(ConvertFileToMusic(wavFileBytes, file.Name));
        }

        return musicDatas;
    }

    private AudioClip ConvertFileToMusic(byte[] wavData, string musicName)
    {
        // WAV 헤더 분석
        int sampleRate = BitConverter.ToInt32(wavData, 24);
        int channels = BitConverter.ToInt16(wavData, 22);
        int byteRate = BitConverter.ToInt32(wavData, 28);
        int dataSize = BitConverter.ToInt32(wavData, 40);
        int sampleCount = dataSize / 2;  // 16bit PCM이므로 2byte당 1 sample

        // WAV 데이터 읽기
        float[] audioData = new float[sampleCount];
        int dataIndex = 44; // 데이터 시작점

        for (int i = 0; i < sampleCount; i++)
        {
            short sample = BitConverter.ToInt16(wavData, dataIndex);
            audioData[i] = sample / 32768.0f; // 16bit PCM을 -1.0f ~ 1.0f 범위로 변환
            dataIndex += 2;
        }

        // AudioClip 생성
        AudioClip audioClip = AudioClip.Create(musicName, sampleCount, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }
}
