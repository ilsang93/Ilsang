using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSelectController : MonoBehaviour
{
    public static BackgroundSelectController Instance { get; private set; }
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject backgroundElementPrefab;
     [SerializeField] private Transform imageListContainer;

    [HideInInspector] public Sprite beforeSprite;
    [HideInInspector] public Sprite nowSprite;

    private const string BACKGROUND_PATH = "/Backgrounds";
    private string backgroundPath;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        backgroundPath = Application.persistentDataPath + BACKGROUND_PATH;

        Initialize();

        closeButton.onClick.AddListener(() =>
        {
            transform.localScale = Vector3.zero;
        });
    }

    private void Initialize()
    {
        List<Sprite> sprites = LoadBackgrounds();

        foreach (Sprite sprite in sprites)
        {
            BackgroundElementController backgroundElement = Instantiate(backgroundElementPrefab, imageListContainer).GetComponent<BackgroundElementController>();
            backgroundElement.Sprite = sprite;
        }
    }

    private List<Sprite> LoadBackgrounds()
    {
        List<Sprite> backgrounds = new();

        DirectoryInfo di = new(backgroundPath);
        if (!di.Exists) di.Create();
        FileInfo[] files = di.GetFiles("*.png");

        foreach (FileInfo file in files)
        {
            byte[] fileData = File.ReadAllBytes(file.FullName);
            Texture2D texture = new(2, 2);
            texture.LoadImage(fileData);

            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            backgrounds.Add(newSprite);
        }

        return backgrounds;
    }
}
