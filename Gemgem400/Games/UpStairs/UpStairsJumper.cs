using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

/// <summary>
/// 미니 게임 "UpStairs"에서 캐릭터가 점프하도록 상태와 위치를 제어하는 클래스
/// </summary>
public class UpStairsJumper : MonoBehaviour
{
    public event Action onChangeStair;

    public UpStairsStair CurStair
    {
        get => curStair;
        set
        {
            if (value != null && IsPlayer == true)
            {
                int curHeight = curStair != null ? curStair.height : 0;
                _upStairsStairManager.MoveStair(value.height - curHeight);
            }
            _animator.SetTrigger("JumpTrigger");
            StartCoroutine(ChildJump());
            transform.DOJump(value.transform.position, 2f,
                1, 0.5f);
            curStair = value;

            onChangeStair?.Invoke();
            var particle = Instantiate(jumpParticle);
            particle.transform.position = this.transform.position;
        }
    }
    private UpStairsStair curStair;
    public UpStairsJumper ChildJumper
    {
        get;
        set;
    }

    public UpStairsJumper ParentJumper
    {
        get;
        set;
    }


    [SerializeField] private ParticleDestroy jumpParticle;

    public Animator _animator;

    private UpStairManager _upStairManager;
    private UpStairsStairManager _upStairsStairManager;
    private bool _isJumping;

    public bool IsJumping
    {
        get => _isJumping;
        private set => _isJumping = value;
    }

    public bool IsPlayer
    {
        get => ParentJumper == null;
    }

    private int Height { get => CurStair.height; }
    private int TargetHeight { get => _upStairManager.TargetHeight; }

    /// <summary>
    /// 플레이어를 추종하는 캐릭터들을 점프시킨다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChildJump()
    {
        yield return new WaitForSeconds(0.2f);
        if (ChildJumper != null)
        {
            ChildJumper.CurStair = CurStair.prevStair;
        }
    }

    private void Awake()
    {
        _upStairManager = FindObjectOfType<UpStairManager>();
        _upStairsStairManager = FindObjectOfType<UpStairsStairManager>();
    }

    public void AddFollower(UpStairsJumper jumper)
    {
        if (CurStair.prevStair == null)
            return;

        if (ChildJumper == null)
        {
            ChildJumper = Instantiate(jumper);
            ChildJumper.transform.position = CurStair.prevStair.transform.position;
            ChildJumper.ParentJumper = this;
            ChildJumper.CurStair = CurStair.prevStair;
        }
        else
        {
            ChildJumper.AddFollower(jumper);
        }
    }

    public void RemoveFollower(string name)
    {
        if ((gameObject.name.Replace("(Clone)", "") == name) && ParentJumper != null)
        {
            RemoveSelf();
        }
        else if (ChildJumper != null)
        {
            ChildJumper.RemoveFollower(name);
        }
    }

    private void RemoveSelf()
    {
        if (ChildJumper != null)
        {
            ChildJumper.ParentJumper = ParentJumper;
            ParentJumper.ChildJumper = ChildJumper;
            ChildJumper.CurStair = curStair;
        }

        Destroy(this.gameObject);
    }

    public void MoveForward(int moveCnt = 1)
    {
        if (IsPlayer)
        {
            Move(moveCnt);
        }
        if (IsPlayer)
        {
            AudioManager.Instance.PlaySfx(Sfxs.CH_Jump);
        }
    }

    public void MoveBackward(int moveCnt = 1)
    {
        if (IsPlayer)
        {
            Move(moveCnt * -1);
        }
        if (IsPlayer)
        {
            AudioManager.Instance.PlaySfx(Sfxs.Trap_05);
        }
    }

    private void Move(int moveCnt)
    {
        if (moveCnt == 0)
            return;
        UpStairsStair selectStair = CurStair;
        // 목표 높이 이상으로 넘어가지 않도록 하는 처리
        if (moveCnt + Height > TargetHeight)
        {
            moveCnt -= (moveCnt + Height) - TargetHeight;
        }

        if (moveCnt > 0)
        {
            // 이벤트나 아이템 박스를 스킵하지 않도록 하는 처리
            for (int i = 1; i <= moveCnt; i++)
            {
                if (selectStair.NextStair == null)
                    break;

                selectStair = selectStair.NextStair;
                if (selectStair.transform.childCount > 1 && IsPlayer)
                {
                    UpStairsItem item = selectStair.transform.GetComponentInChildren<UpStairsItem>();
                    if (item != null)
                    {
                        Debug.Log("없다");
                        _upStairManager.GameState = UpStairManager.UpStairState.Wait;
                    }

                    moveCnt = i;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i > moveCnt; i--)
            {
                if (selectStair.prevStair == null)
                    break;
                selectStair = selectStair.prevStair;
            }
        }
        CurStair = selectStair;
    }

    public void EndingMotion()
    {
        StartCoroutine(CR_EndingMotion());
    }

    private IEnumerator CR_EndingMotion()
    {
        _animator.SetBool("IsWalk", true);
        this.transform.DOMoveX(this.transform.position.x + 10.0f, 3).OnComplete(() =>
        {
            if (ChildJumper == null)
                _upStairManager.GameClear();
        });
        if (ChildJumper != null)
        {
            ChildJumper.CurStair = CurStair;
        }
        yield return new WaitForSeconds(0.5f);
        if (ChildJumper != null)
        {
            ChildJumper.EndingMotion();
        }
    }
}