using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Transform trailerTransform;

    void Update()
    {
        if (trailerTransform == null) return;
        transform.position = GetParalaxPosition();
    }

    private Vector3 GetParalaxPosition()
    {
        return new Vector2(trailerTransform.position.x * 0.05f, trailerTransform.position.y * 0.05f);
    }
}
