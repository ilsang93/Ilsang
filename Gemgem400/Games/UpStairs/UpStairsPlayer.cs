using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using System;

/// <summary>
/// 미니 게임 "UpStair"의 플레이어 오브젝트를 제어하는 클래스
/// </summary>
public class UpStairsPlayer : MonoBehaviour
{
    public UpStairsJumper playerJumper;
    
    private UpStairManager _upStairManager;
    
    private float jumpCoolTime = 0.15f;
    private float curCoolTime = 0.0f;

    [SerializeField]private SpriteRenderer egg;
    private bool IsJumping
    {
        get;
        set;
    }

    private void Start()
    {
        _upStairManager = FindObjectOfType<UpStairManager>();
    }

    private void Update()
    {
        if (curCoolTime >= jumpCoolTime)
        {
            curCoolTime = 0.0f;
            IsJumping = false;
        }
        if (IsJumping == true)
            curCoolTime += UnityEngine.Time.deltaTime;
        
    }
    
    public void AddFollower(UpStairsJumper jumper)
    {
        playerJumper.AddFollower(jumper);
    }

    public void RemoveFollower(string name)
    {
        playerJumper.RemoveFollower(name);
    }

    public void GetInput(bool input)
    {
        if (IsJumping == false && input == true)
        {
            IsJumping = true;
            MoveForward(1);
        }
    }
    
    public void MoveForward(int moveCnt = 1)
    {
        playerJumper.MoveForward(moveCnt);
    }
    
    public void MoveBackward(int moveCnt = 1)
    {
        playerJumper.MoveBackward(moveCnt);
    }
    
    public void EndingMotion()
    {
        StartCoroutine(CR_EndingMotion());
    }

    private IEnumerator CR_EndingMotion()
    {
        EggOnHead();
        yield return new WaitForSeconds(1.0f);
        playerJumper._animator.SetTrigger("JumpTrigger");
        yield return new WaitForSeconds(0.5f);
        playerJumper._animator.SetTrigger("JumpTrigger");
        yield return new WaitForSeconds(0.5f);
        playerJumper._animator.SetTrigger("JumpTrigger");
        yield return new WaitForSeconds(1.0f);
        playerJumper.EndingMotion();
    }

    public void EggOnHead()
    {
        egg.transform.gameObject.SetActive(true);
        AudioManager.Instance.PlaySfx(Sfxs.RandomBox_CH_Add);
        egg.transform.DOLocalJump(new Vector3(egg.transform.localPosition.x, egg.transform.localPosition.y,egg.transform.localPosition.z), 0.5f, 1, 0.5f).SetEase(Ease.Linear).SetLoops(-1); ;
        egg.DOColor(Color.white, 0.25f);
    }
}