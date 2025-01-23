using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class DataConvertUtils
{
    // 음원 클립을 Base64 문자열로 변환
    internal static string ConvertMusicToBase64(AudioClip musicClip)
    {
        // 오디오 클립의 길이만큼의 배열을 생성
        float[] samples = new float[musicClip.samples * musicClip.channels]; // 채널 수 반영
        musicClip.GetData(samples, 0);

        // 메모리 스트림과 바이너리 라이터 생성
        MemoryStream stream = new();
        BinaryWriter writer = new(stream);

        // 오디오 클립의 샘플레이트와 채널 수를 먼저 기록
        writer.Write(musicClip.frequency);
        writer.Write(musicClip.channels); // 채널 수 저장

        // 샘플 데이터를 바이너리로 쓴다.
        foreach (float sample in samples)
        {
            writer.Write(sample);
        }

        // 스트림을 바이트 배열로 변환
        byte[] musicData = stream.ToArray();
        // 바이트 배열을 Base64 문자열로 변환
        return Convert.ToBase64String(musicData);
    }

    // Base64 문자열을 음원 클립으로 변환
    internal static AudioClip ConvertBase64ToMusic(string base64str)
    {
        AudioClip resultClip = null;

        // Base64 문자열을 바이트 배열로 변환
        byte[] musicData = Convert.FromBase64String(base64str);
        // 바이트 배열을 메모리 스트림으로 변환
        MemoryStream stream = new(musicData);
        // 바이너리 리더 생성
        BinaryReader reader = new(stream);

        // 먼저 저장된 샘플레이트와 채널 수를 읽는다
        int frequency = reader.ReadInt32();
        int channels = reader.ReadInt32(); // 채널 수 읽기

        // 샘플 데이터를 담을 리스트 생성
        List<float> samples = new();
        // 바이너리 리더로 샘플 데이터를 읽어 리스트에 추가
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            samples.Add(reader.ReadSingle());
        }

        // 샘플 데이터로 오디오 클립 생성 (채널 수를 원래 채널 수로 맞추기)
        resultClip = AudioClip.Create("Music", samples.Count / channels, channels, frequency, false);
        resultClip.SetData(samples.ToArray(), 0);

        return resultClip;
    }

    // 스프라이트를 Base64 문자열로 변환
    internal static string ConvertSpriteToBase64(Sprite sprite)
    {
        Texture2D texture = sprite.texture;
        byte[] bytes = texture.EncodeToPNG();
        return Convert.ToBase64String(bytes);
    }

    // Base64 문자열을 스프라이트로 변환
    internal static Sprite ConvertBase64ToSprite(string base64str)
    {
        byte[] bytes = Convert.FromBase64String(base64str);
        Texture2D texture = new(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
