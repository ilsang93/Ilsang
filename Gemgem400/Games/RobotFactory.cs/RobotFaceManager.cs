using UnityEngine;

/// <summary>
/// 낙하하는 로봇 머리 오브젝트의 상태를 제어하는 클래스.
/// </summary>
public class RobotFaceManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer robotFace;
    [SerializeField] private Sprite correctFace;
    public bool correctFlg;
    public bool canInputFlg;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Equals("CorrectLineSystem"))
        {
            correctFlg = true;
        }
        else if (other.gameObject.name.Equals("CanInputLineSystem"))
        {
            canInputFlg = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "CorrectLineSystem")
        {
            correctFlg = false;
        }
    }

    public void ChangeFaceSpriteToCorrect()
    {
        robotFace.sprite = correctFace;
    }
}