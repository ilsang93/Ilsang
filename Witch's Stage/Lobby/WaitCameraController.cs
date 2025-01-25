using UnityEngine;

public class WaitCameraController : MonoBehaviour
{
    [SerializeField] private Vector3 upperLeft;
    [SerializeField] private Vector3 lowerRight;
    [SerializeField] private float speed;
    private CameraPosState cameraPosState = CameraPosState.UpperLeft;

    void Start()
    {
        transform.position = upperLeft;
    }

    void Update()
    {
        Vector3 targetPos = Vector3.zero;
        switch (cameraPosState)
        {
            case CameraPosState.UpperLeft:
                targetPos = upperLeft;
                break;
            case CameraPosState.UpperRight:
                targetPos = new Vector3(lowerRight.x, upperLeft.y, -10f);
                break;
            case CameraPosState.LowerRight:
                targetPos = lowerRight;
                break;
            case CameraPosState.LowerLeft:
                targetPos = new Vector3(upperLeft.x, lowerRight.y, -10f);
                break;
        }

        transform.Translate(speed * Time.deltaTime * (targetPos - transform.position).normalized);

        switch (cameraPosState)
        {
            case CameraPosState.UpperLeft:
                if (transform.position.x >= upperLeft.x && transform.position.y >= upperLeft.y)
                {
                    cameraPosState = CameraPosState.UpperRight;
                }
                break;
            case CameraPosState.UpperRight:
                if (transform.position.x >= lowerRight.x && transform.position.y <= upperLeft.y)
                {
                    cameraPosState = CameraPosState.LowerRight;
                }
                break;
            case CameraPosState.LowerRight:
                if (transform.position.x <= lowerRight.x && transform.position.y <= lowerRight.y)
                {
                    cameraPosState = CameraPosState.LowerLeft;
                }
                break;
            case CameraPosState.LowerLeft:
                if (transform.position.x <= upperLeft.x && transform.position.y >= lowerRight.y)
                {
                    cameraPosState = CameraPosState.UpperLeft;
                }
                break;
        }
    }

    private enum CameraPosState
    {
        UpperLeft,
        UpperRight,
        LowerRight,
        LowerLeft
    }
}
