using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    [SerializeField] private GameObject stickSpriteObject;
    [SerializeField] private ParticleSystem stickParticle;
    private Queue<Vector2> characterPosQueue = new();
    private float sinTime = 0;

    void Update()
    {
        if (Vector2.Distance(StageManager.Instance.CharacterController.transform.position, transform.position) > 2f)
        {
            transform.Translate(StageManager.Instance.CharacterController.speed * Time.deltaTime * (StageManager.Instance.CharacterController.transform.position - transform.position).normalized);
        }

        // 부모 오브젝트의 방향과 관계 없이 항상 똑바로 표시되도록 한다.
        stickSpriteObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        stickSpriteObject.transform.localPosition = new Vector2(stickSpriteObject.transform.localPosition.x, Mathf.Sin(sinTime += Time.deltaTime) * 0.5f);
    }
}
