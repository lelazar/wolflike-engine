using System;

namespace WolfLike.src.Core;

public static class GameSettings
{
    public const int SCREEN_WIDTH = 1280;
    public const int SCREEN_HEIGHT = 720;

    public const int RAY_COUNT = 320;

    public const float FIELD_OF_VIEW = MathF.PI / 3.0f;  // 60 degrees

    public const float MAX_RAY_DISTANCE = 20.0f;

    public const int TEXTURE_SIZE = 64;

    public const string DEFAULT_LEVEL_FILE = "level01.txt";
}
