using UnityEngine;

public class StageSelectConstant
{
    public static readonly Color32[] ORIGINAL_LEVEL_COLORS = new Color32[] {
        new() { r = 150, g = 220, b = 150, a = 255 },
        new() { r = 80, g = 100, b = 255, a = 255 },
        new() { r = 200, g = 30, b = 30, a = 255 },
        new() { r = 210, g = 60, b = 190, a = 255 },
    };

    public static readonly Color32 NO_LEVEL_COLOR = new() { r = 100, g = 100, b = 100, a = 255 };
}