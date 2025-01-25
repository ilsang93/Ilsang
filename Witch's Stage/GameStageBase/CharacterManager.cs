using DG.Tweening;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager inputManager = FindObjectOfType<InputManager>();
    }

    private void MoveLeft()
    {
        if (PlayerManager.instance.chracterStatus == PlayerManager.ChracterStatus.Idle)
        {
            PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Move;
        }
        else
        {
            return;
        }
        transform.DOJump(transform.position + Vector3.left, 0.5F, 1, 0.5f);
        PlayerManager.instance.transform.DOLocalMove(PlayerManager.instance.transform.localPosition + Vector3.left, 0.5f).OnComplete(() => PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Idle);
        NotesManager.instance.DoJudgement();
    }

    private void MoveRight()
    {
        if (PlayerManager.instance.chracterStatus == PlayerManager.ChracterStatus.Idle)
        {
            PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Move;
        }
        else
        {
            return;
        }
        transform.DOJump(transform.position + Vector3.right, 0.5F, 1, 0.5f);
        PlayerManager.instance.transform.DOLocalMove(PlayerManager.instance.transform.localPosition + Vector3.right, 0.5f).OnComplete(() => PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Idle);
        NotesManager.instance.DoJudgement();
    }

    private void MoveUp()
    {
        if (PlayerManager.instance.chracterStatus == PlayerManager.ChracterStatus.Idle)
        {
            PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Move;
        }
        else
        {
            return;
        }
        transform.DOJump(transform.position + Vector3.up, 0.5F, 1, 0.5f);
        PlayerManager.instance.transform.DOLocalMove(PlayerManager.instance.transform.localPosition + Vector3.up, 0.5f).OnComplete(() => PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Idle);
        NotesManager.instance.DoJudgement();
    }

    private void MoveDown()
    {
        if (PlayerManager.instance.chracterStatus == PlayerManager.ChracterStatus.Idle)
        {
            PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Move;
        }
        else
        {
            return;
        }
        transform.DOJump(transform.position + Vector3.down, 0.5F, 1, 0.5f);
        PlayerManager.instance.transform.DOLocalMove(PlayerManager.instance.transform.localPosition + Vector3.down, 0.5f).OnComplete(() => PlayerManager.instance.chracterStatus = PlayerManager.ChracterStatus.Idle);
        NotesManager.instance.DoJudgement();
    }
}
