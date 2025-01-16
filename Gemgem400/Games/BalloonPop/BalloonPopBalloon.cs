using DG.Tweening;
using UnityEngine;

/// <summary>
/// 각 풍선 객체를 컨트롤하는 클래스
/// </summary>
public class BalloonPopBalloon : MonoBehaviour
{
    [SerializeField] private GameObject particleEffect;
    [SerializeField] private AudioClip _audioClip;

    public void Pop()
    {
        transform.DOShakeScale(0.3f, 1f).OnComplete(() =>
        {
            GameObject p = Instantiate(particleEffect, transform.position, Quaternion.identity);
            p.SetActive(true);
            if (_audioClip == null)
            {
                AudioManager.Instance.PlaySfx(Sfxs.MP_BalloonPopping);
            }
            else
            {
                AudioManager.Instance.PlaySfx(_audioClip);
            }

            Destroy(gameObject);
        });
    }
}