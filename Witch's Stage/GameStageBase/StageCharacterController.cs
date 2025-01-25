using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class StageCharacterController : MonoBehaviour
{

    [SerializeField] private TextMeshPro distanceText;
    [SerializeField] private GameObject judgeTrailer;
    [SerializeField] private GameObject mouseDirectionObject;
    [SerializeField] private GameObject stickObject;
    [SerializeField] private SpriteRenderer characterRenderer;

    [HideInInspector] public bool warningFlg = false;
    [HideInInspector] public float speed = 1f;

    public Vector2 direction { get; private set; }
    public float holdSpeed = 0.1f;
    public float minSpeed, maxSpeed;
    public float regainTime = 2f;

    private float originSpeed;
    private bool damaged;
    private float damgedTime = 0f;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        originSpeed = StageManager.Instance.StageData.speedMultiplier * StageConstant.DEFAULT_SPEED;

        minSpeed = originSpeed - (originSpeed * 0.5f);
        maxSpeed = originSpeed + (originSpeed * 0.5f);
        holdSpeed = originSpeed * 0.1f;
        damaged = false;

        speed = originSpeed;

        inputManager = FindObjectOfType<InputManager>();

        inputManager.OnHoldMouseLeft += HoldLeftMouseButton;
        inputManager.OnHoldMouseRight += HoldRightMouseButton;
        inputManager.OnHoldMouseBoth += ReleaseMouseButton;
        inputManager.OnReleaseMouseBoth += ReleaseMouseButton;
    }

    // Update is called once per frame
    void Update()
    {
        // 항상 작동하는 코드
        mouseDirectionObject.transform.up = GetMouseDirection();

        // 게임 시작 이후 작동할 코드
        if (StageManager.Instance.GameStarted)
        {
            // 데미지 이벤트 중에는 처리를 생략한다.
            if (damaged)
            {
                damgedTime = 0f;
                return;
            }

            damgedTime += Time.deltaTime;

            // 데미지를 받고 regainTime이 지날때 마다 체력을 회복한다.
            if (damgedTime > regainTime)
            {
                StageManager.Instance.NowHP += 0.1f;
                damgedTime = 0f;
            }

            StageManager.Instance.JudgeDistance = Vector2.Distance(judgeTrailer.transform.position, transform.position);

            // 판정점과의 거리를 구한다.
            if (StageManager.Instance.JudgeDistance < StageConstant.SAFE_DISTANCE)
            {
                distanceText.text = "좋아!";
            }
            else
            {
                distanceText.text = "멀어!";
            }

            if (StageManager.Instance.JudgeDistance < StageConstant.SAFE_DISTANCE * 1.25f)
            {
                transform.Translate(speed * Time.deltaTime * GetMouseDirection());
            }
            else
            {
                damaged = true;
                StartCoroutine(Damaged());
            }
        }
        else
        {
            distanceText.text = "준비!";
        }
    }

    private IEnumerator Damaged()
    {
        characterRenderer.DOColor(Color.red, 0.1f).SetLoops(10, LoopType.Yoyo);
        float time = 0f;
#if UNITY_EDITOR

#else
        StageManager.Instance.NowHP -= 1f;
#endif
        while (true)
        {
            time += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, judgeTrailer.transform.position, time * 2);
            if (time > 0.5f) break;
            yield return null;
        }
        characterRenderer.color = Color.white;
        characterRenderer.DOKill();
        damaged = false;
    }

    private Vector2 GetMouseDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 targetDirection = mousePosition - (Vector2)transform.position;
        direction = targetDirection.normalized;
        if (!StageManager.Instance.GameStarted)
        {
            characterRenderer.flipX = true;
        }
        else
        {
            if (direction.x >= 0)
            {
                characterRenderer.flipX = true;
            }
            else
            {
                characterRenderer.flipX = false;
            }
        }

        return targetDirection.normalized;
    }

    private void HoldLeftMouseButton()
    {

        if (speed < maxSpeed) speed += holdSpeed;
    }

    private void HoldRightMouseButton()
    {
        if (speed > minSpeed) speed -= holdSpeed;
    }

    private void ReleaseMouseButton()
    {
        if (originSpeed > speed) speed += holdSpeed;
        else if (originSpeed < speed) speed -= holdSpeed;
    }
}
