using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public enum Sfxs
{
    Btn_Common,
    Btn_RandomBox,
    CH_Jump,
    CH_Landing,
    Coin,
    GameOver,
    IntroLogo,
    IntroLogo_in,
    IntroLogo_out,
    RandomBox_CH_Add,
    RandomBox_Drop,
    RandomBox_Open,
    RandomBoxTap,
    ReadyGo,
    Result,
    Transition_in,
    Transition_Out,
    Trap_01,
    Trap_02,
    Trap_03,
    Trap_04_01,
    Trap_04_02,
    Trap_05,
    MP_BalloonPopping,
    ArrowImpactwood4,
    Winsound1,
    Lostsound1,
    SciFiBeamReload1,
    Buff2,
    BodyDrop5,
    water_splash_small_item_01,
    rock_impact_heavy_slam_03
}

public enum Bgms
{
    CasualBonusLevelDanger,
    GetUpFull,
    RedNBlueLoopable,
    VillageLoopable,
    HopeLoopable,
    WholesomeLoopable,
    FriendsLoopable,
    SkydivingLoopable,
    TogetherLoopable,
    LOGIN_Preparing_for_Adventure,
    HOME_Remembering_A_Better_Time,
    STRETCH_LOOP_Gonna_Be_A_Good_Day,
    PROGRESS_Adventure_On_the_Horizon
}



public class AudioManager : MonoBehaviour
{
    private AudioClip currentBgm;

    public AudioClip CurrentBgm
    {
        get => currentBgm;
    }
    
    public static AudioManager Instance;

    [SerializeField] private AudioClipByEnum<Bgms>[] bgms;
    [SerializeField] private AudioClipByEnum<Sfxs>[] sfxs;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource bgmaudioSource;
    private readonly Dictionary<Bgms, AudioClip> bgmClips = new();

    private readonly Dictionary<Sfxs, AudioClip> sfxClips = new();

    float defaultBgmVolume = 0.25f;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Init();
    }

    private void Init()
    {
        foreach (AudioClipByEnum<Sfxs> s in sfxs)
        {
            sfxClips.TryAdd(s.name, s.clip);
        }

        foreach (AudioClipByEnum<Bgms> b in bgms)
        {
            bgmClips.TryAdd(b.name, b.clip);
        }
    }

    public void PlaySfx(Sfxs _playSfx)
    {
        if (sfxClips.ContainsKey(_playSfx))
        {
            audioSource.PlayOneShot(sfxClips[_playSfx]);
        }
    }

    public void PlaySfx(AudioClip sfxClip)
    {
        audioSource.PlayOneShot(sfxClip);
    }

    public void PlayBgm(Bgms _playBgm)
    {
        if (bgmClips.ContainsKey(_playBgm))
        {
            currentBgm = bgmClips[_playBgm];
            bgmaudioSource.volume = defaultBgmVolume;
            bgmaudioSource.Stop();
            bgmaudioSource.clip = bgmClips[_playBgm];
            bgmaudioSource.Play();
        }
    }

    public void PlayBgm(AudioClip bgmClip)
    {
        if (bgmClip != null)
        {
            currentBgm = bgmClip;
            bgmaudioSource.volume = defaultBgmVolume;
            bgmaudioSource.Stop();
            bgmaudioSource.clip = bgmClip;
            bgmaudioSource.Play();
        }
    }

    public void StopBgms(Action _stopAction = null, float _duration = 1.0f)
    {
        StartCoroutine(CR_StopBgm(_stopAction, _duration));
    }

    IEnumerator CR_StopBgm(Action _stopAction = null, float _duration = 1.0f)
    {
        while (bgmaudioSource.volume > 0)
        {
            bgmaudioSource.volume -= Time.deltaTime / _duration * defaultBgmVolume;
            yield return null;
        }
        bgmaudioSource.Stop();
        yield return new WaitForSeconds(1.0f);
        _stopAction?.Invoke();
    }

    [Serializable]
    private class AudioClipByEnum<T> where T : Enum
    {
        [SerializeField] public T name;
        [SerializeField] public AudioClip clip;
    }
}