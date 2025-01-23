using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;
    [HideInInspector] public bool isLoading = false;
    [SerializeField] private SceneLoadController sceneLoadController;
    [SerializeField] private Canvas canvas;

    public TransferStageData StageData { get => stageData; set => stageData = value; }
    private TransferStageData stageData;

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

    void Update()
    {
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    public void ReloadScene()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        //TODO 씬 로드를 시작하기 전에 로딩 연출을 표시한다.
        yield return StartCoroutine(CloseSceneForLoad());
        SoundManager.Instance?.StopBGM();
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);

        ao.allowSceneActivation = false;
        yield return new WaitForSeconds(1f);

        while (ao == null || ao.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        ao.allowSceneActivation = true;

        StopAllCoroutines();
        //TODO 씬 로드가 완료되면 로딩 연출을 비표시한다.
        yield return StartCoroutine(OpenSceneForLoad());
    }

    public IEnumerator LoadStageScene(TransferStageData stageData)
    {
        //TODO 씬 로드를 시작하기 전에 로딩 연출을 표시한다.
        yield return StartCoroutine(CloseSceneForLoad());

        StageData = stageData;

        SoundManager.Instance?.StopBGM();
        AsyncOperation ao = SceneManager.LoadSceneAsync("GameStageBase");

        ao.allowSceneActivation = false;
        yield return new WaitForSeconds(1f);

        while (ao == null || ao.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        ao.allowSceneActivation = true;

        StopAllCoroutines();
        // 씬 로드가 완료되면 로딩 연출을 비표시한다.
        yield return StartCoroutine(OpenSceneForLoad());
    }

    /// <summary>
    /// 씬을 로드하기 전, 씬이 닫히는 연출을 표시한다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator CloseSceneForLoad()
    {
        isLoading = true;
        yield return StartCoroutine(sceneLoadController.Close());
    }

    /// <summary>
    /// 씬이 로드된 후, 씬이 열리는 연출을 표시한다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator OpenSceneForLoad()
    {
        yield return StartCoroutine(sceneLoadController.Open());
        isLoading = false;
    }

}
