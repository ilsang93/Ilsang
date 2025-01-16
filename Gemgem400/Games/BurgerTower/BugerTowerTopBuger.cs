using UnityEngine;

/// <summary>
/// 버거가 완성된 경우 출현하는 뚜껑빵이 소리를 재생하도록 하는 클래스
/// </summary>
public class BugerTowerTopBuger : MonoBehaviour
{
    private bool soundPlay;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (soundPlay == false)
        {
            AudioManager.Instance.PlaySfx(Sfxs.CH_Landing);
            soundPlay = true;
        }
    }
}