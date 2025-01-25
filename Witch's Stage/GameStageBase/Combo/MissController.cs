using UnityEngine;
using DG.Tweening;

public class MissController : MonoBehaviour
{
    private Vector3 originPos;

    public void Start()
    {
        originPos = transform.localPosition;
    }
    public void Miss()
    {
        transform.localPosition = originPos;

        // 콤보 텍스트 애니메이션션
        transform.DOKill();
        transform.DOLocalJump(originPos, 5f, 1, 0.5f).SetEase(Ease.OutElastic);
    }
}