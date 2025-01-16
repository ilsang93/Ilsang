using UnityEngine;
public class GoalKeeperBallController : MonoBehaviour
{
    public bool keepedFlg;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name.Equals("Player"))
        {
            AudioManager.Instance.PlaySfx(Sfxs.RandomBoxTap);
            keepedFlg = true;
        }
    }
}