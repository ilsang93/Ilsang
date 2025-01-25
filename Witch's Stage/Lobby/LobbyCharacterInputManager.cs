using System;
using UnityEngine;

public class LobbyCharacterInputManager : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    internal Action onPressEnter;
    internal Action onPressA;
    internal Action onPressD;
    internal Action onPressW;
    internal Action onPressS;
    internal Action onPressESC;
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;

    private float waitTime = 0f;
#if UNITY_EDITOR
    private const float targetWaitTime = 5f;
#else
    private const float targetWaitTime = 45f;
#endif


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        onPressA += () =>
        {
            rb.velocity += Vector2.left * speed;
        };
        onPressD += () =>
        {
            rb.velocity += Vector2.right * speed;
        };
        onPressW += () =>
        {
            rb.velocity += Vector2.up * speed;
        };
        onPressS += () =>
        {
            rb.velocity += Vector2.down * speed;
        };
    }

    // Update is called once per frame
    void Update()
    {
        bool isWait = true;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            onPressEnter?.Invoke();
            isWait = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            onPressESC?.Invoke();
            isWait = false;
        }

        rb.velocity = Vector2.zero;

        if (LobbyManager.Instance.lobbyState != LobbyState.None)
        {
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            onPressA?.Invoke();
            isWait = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            onPressD?.Invoke();
            isWait = false;
        }
        if (Input.GetKey(KeyCode.W))
        {
            onPressW?.Invoke();
            isWait = false;
        }
        if (Input.GetKey(KeyCode.S))
        {
            onPressS?.Invoke();
            isWait = false;
        }

        if (rb.velocity != Vector2.zero)
        {
            if (rb.velocity.x < 0)
            {
                sr.flipX = true;
            }
            else if (rb.velocity.x > 0)
            {
                sr.flipX = false;
            }
        }

        if (isWait)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= targetWaitTime)
            {
                LobbyManager.Instance.ChangeCameraToWait();
            }
        }
        else
        {
            if (waitTime >= targetWaitTime)
            {
                LobbyManager.Instance.ChangeCameraToCharacter();
            }
            waitTime = 0f;
        }
    }
}
