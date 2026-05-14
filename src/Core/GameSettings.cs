using System;

namespace WolfLike.src.Core;

public static class GameSettings
{
    public const int SCREENWIDTH = 1280;
    public const int SCREENHEIGHT = 720;

    public const int RAYCOUNT = 320;

    public const float FIELDOFVIEW = MathF.PI / 3.0f;  // 60 degrees

    public const float MAXRAYDISTANCE = 20.0f;

    public const int TEXTURESIZE = 64;
}
