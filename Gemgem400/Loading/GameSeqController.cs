using Spine.Unity;

using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GemgemAr;
using Spine;
using Event = Spine.Event;

/// <summary>
/// 로딩 중 게임 진행 단계를 제어하기 위한 게임 진행 단계를 관리한다.
/// </summary>
public class GameSeqController : MonoBehaviour
{
    public static GameSeqController Instance { get; private set; }
    [SerializeField]
    private GameObject progressPanel;
    [SerializeField]
    private GameObject stepModule;
    [SerializeField]
    private GameObject[] stepArray;
    [SerializeField]
    private GameObject moduleGroup;
    [SerializeField]
    private AudioSource progressAudioSource;

    private int currentProgress;
    private bool getEggEndFlg = true;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void GetEggEnd()
    {
        getEggEndFlg = true;
    }

    public IEnumerator GoNextStep()
    {
        currentProgress++;
        yield break;
    }
}