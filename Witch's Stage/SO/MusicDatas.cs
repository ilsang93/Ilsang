using UnityEngine;

public class MusicDatas
{
    public string musicName;
    public AudioClip musicClip;

    public MusicDatas(string musicName, AudioClip musicClip)
    {
        this.musicName = musicName;
        this.musicClip = musicClip;
    }
}
