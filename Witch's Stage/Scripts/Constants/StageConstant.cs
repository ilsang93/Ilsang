
using UnityEngine;

public static class StageConstant
{
    public const float NODE_DISTANCE = 10.0f; // 노드 간 거리
    public const float NODE_TIME = 1f; // 노드 간 이동 시간

    public const float RAIL_WIDTH = 0.5f; // 레일의 길이
    public const float DEFAULT_SPEED = 8;
    public const float JUDGE_TIME_UNIT = 0.01667f;
    public const float SAFE_DISTANCE = 6f; // 안전 거리

    public const string STAGE_DATA_PATH = "/StageData"; // 스테이지 데이터 저장 경로

    public static readonly Vector2 FULL_DELTA_SIZE = new(1920, 1080);
    public static readonly Vector2 VECTOR2_ZERO = Vector2.zero;
    public static readonly Vector2 VECTOR2_ONE = Vector2.one;

    public static readonly Color32 COLOR32_SPOTLIGHT_1 = new(255, 200, 0, 20);
    public static readonly Color32 COLOR32_SPOTLIGHT_2 = new(255, 200, 0, 40);
    public static readonly Color32 COLOR32_SPOTLIGHT_3 = new(255, 200, 0, 80);
    public static readonly Color32 COLOR32_SAFEAREA = new(60, 255, 0, 80);
    public static readonly Color32 COLOR32_WHITE = new(255, 255, 255, 255);
    public static readonly Color32 COLOR32_BLACK = new(0, 0, 0, 255);
    public static readonly Color32 COLOR32_CLEAR = new(255, 255, 255, 0);

    public const float LENS_ORTH_SIZE_FAR = 80f;
    public const float LENS_ORTH_SIZE_ZOOM_1 = 70f;
    public const float LENS_ORTH_SIZE_ZOOM_2 = 50f;
    public const float LENS_ORTH_SIZE_DEFAULT = 30f;
}
