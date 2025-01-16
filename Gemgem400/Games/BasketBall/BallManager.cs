using UnityEngine;

/// <summary>
/// 던져진 공 오브젝트를 관리하는 클래스
/// </summary>
public class BallManager : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip _hitSfxClip;
    public bool goalFlg { get; set; }
    public bool downFlg { get; set; } = false;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_hitSfxClip != null)
        {
            AudioManager.Instance.PlaySfx(_hitSfxClip);
        }
        else
        {
            AudioManager.Instance.PlaySfx(Sfxs.RandomBox_Drop);
        }
        if (hitEffect != null)
        {
            Instantiate(hitEffect).transform.position = this.transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (downFlg)
        {
            if (other.name == "GoalPoint")
            {
                goalFlg = true;
            }
        }
    }
}