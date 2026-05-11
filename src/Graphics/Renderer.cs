using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using WolfLike.src.Entities;
using WolfLike.src.World;

namespace WolfLike.src.Graphics;

public class Renderer
{
    private Texture2D _pixel;

    private const int TILESIZE = 48;

    private const int SCREENWIDTH = 1280;
    private const int SCREENHEIGHT = 720;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public void DrawRaycastView(SpriteBatch spriteBatch, WorldMap worldMap, Player player, RaycastHit[] rayHits)
    {
        spriteBatch.Begin(blendState: BlendState.AlphaBlend);

        DrawCeilingAndFloor(spriteBatch);
        DrawWallSlices(spriteBatch, player, rayHits);

        // Small debug minimap in the top-left corner
        //DrawMiniMap(spriteBatch, worldMap, player, rayHits);

        spriteBatch.End();
    }

    private void DrawCeilingAndFloor(SpriteBatch spriteBatch)
    {
        Rectangle ceilingRectangle = new Rectangle(0, 0, SCREENWIDTH, SCREENHEIGHT / 2);
        Rectangle floorRectangle = new Rectangle(0, SCREENHEIGHT / 2, SCREENWIDTH, SCREENHEIGHT / 2);

        spriteBatch.Draw(_pixel, ceilingRectangle, new Color(22, 24, 38));
        spriteBatch.Draw(_pixel, floorRectangle, new Color(38, 38, 42));
    }

    private void DrawWallSlices(SpriteBatch spriteBatch, Player player, RaycastHit[] rayHits)
    {
        if (rayHits == null || rayHits.Length == 0) return;

        int rayCount = rayHits.Length;
        float sliceWidth = (float)SCREENWIDTH / rayCount;  // Controls how wide each vertical slice is. If SCREENWIDTH=1280, rayCount=320 then sliceWidth = 4 pixels
        // So every ray draws a 4-pixel-wide vertical wall column

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit rayHit = rayHits[i];

            if (rayHit == null || !rayHit.HitWall) continue;

            // Without correction, the left and right sides of the screen look distorted because those rays travel diagonally
            // So let's correct the distance with fish-eye correction
            float correctedDistance = CorrectFishEye(rayHit.Distance, rayHit.Angle, player.Angle);  // This avoids the classic fish-eye distortion
            // Without it, a flat wall in front of us may look curved. With it, walls look much more stable

            if (correctedDistance < 0.0001f)
                correctedDistance = 0.0001f;

            // The heart of the renderer
            float wallHeight = SCREENHEIGHT / correctedDistance;  // Objects closer to the camera appear larger. For example dist1 -> 720/1 = 720 pixels tall, dist2 -> 720/2 = 360 pixels tall
            // With this, we can create the 3D illusion

            int sliceX = (int)(i * sliceWidth);
            int sliceHeight = (int)wallHeight;
            int sliceY = SCREENHEIGHT / 2 - sliceHeight / 2;

            Rectangle wallSliceRectangle = new Rectangle(sliceX, sliceY, (int)MathF.Ceiling(sliceWidth) + 1, sliceHeight);

            Color wallColor = GetWallColor(rayHit, correctedDistance);

            spriteBatch.Draw(_pixel, wallSliceRectangle, wallColor);
        }
    }

    private float CorrectFishEye(float rawDistance, float rayAngle, float playerAngle)
    {
        float angleDifference = rayAngle - playerAngle;

        return rawDistance * MathF.Cos(angleDifference);
    }

    private Color GetWallColor(RaycastHit rayHit, float correctedDistance)
    {
        Color baseColor = GetBaseWallColor(rayHit.TileId);

        float sideBrightness = rayHit.HitSide switch
        {
            WallHitSide.Vertical => 1.00f,
            WallHitSide.Horizontal => 0.72f,
            _ => 1.00f
        };

        float distanceBrightness = GetDistanceBrightness(correctedDistance);

        float finalBrightness = sideBrightness * distanceBrightness;

        return ApplyBrightness(baseColor, finalBrightness);
    }

    private Color GetBaseWallColor(int tileId)
    {
        return tileId switch
        {
            1 => new Color(170, 170, 190),
            2 => new Color(170, 80, 80),
            3 => new Color(80, 140, 190),
            _ => new Color(200, 200, 200)
        };
    }

    private float GetDistanceBrightness(float distance)
    {
        float brightness = 1.0f / (1.0f + distance * 0.12f);

        return MathHelper.Clamp(brightness, 0.18f, 1.0f);
    }

    private Color ApplyBrightness(Color color, float brightness)
    {
        brightness = MathHelper.Clamp(brightness, 0.0f, 1.0f);

        return new Color(
            (byte)(color.R * brightness),
            (byte)(color.G * brightness),
            (byte)(color.B * brightness),
            color.A
        );
    }

    private void DrawMiniMap(SpriteBatch spriteBatch, WorldMap worldMap, Player player, RaycastHit[] rayHits)
    {
        DrawMap(spriteBatch, worldMap);
        DrawRays(spriteBatch, player, rayHits);
        DrawPlayer(spriteBatch, player);
        DrawPlayerDirection(spriteBatch, player);
    }

    private void DrawMap(SpriteBatch spriteBatch, WorldMap worldMap)
    {
        for (int y = 0; y < worldMap.Height; y++)
        {
            for (int x = 0; x < worldMap.Width; x++)
            {
                int tile = worldMap.GetTile(x, y);

                Color color = tile switch
                {
                    0 => new Color(35, 35, 35, 220),
                    1 => new Color(180, 180, 180, 220),
                    2 => new Color(180, 80, 80, 220),
                    3 => new Color(80, 140, 190, 220),
                    _ => Color.Magenta
                };

                Rectangle tileRectangle = new Rectangle(
                    x * TILESIZE,
                    y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                );

                spriteBatch.Draw(_pixel, tileRectangle, color);

                DrawRectangleBorder(spriteBatch, tileRectangle, Color.Black);
            }
        }
    }

    private void DrawRays(SpriteBatch spriteBatch, Player player, RaycastHit[] rayHits)
    {
        if (rayHits == null) return;

        Vector2 start = WorldToScreen(player.Position);

        int centerIndex = rayHits.Length / 2;

        // At the moment, the center ray cannot be seen, so modify it below

        // So this gives me green FOV rays and yellow center ray
        for (int i = 0; i < rayHits.Length; i++)
        {
            RaycastHit rayHit = rayHits[i];

            if (rayHit == null) continue;

            Vector2 end = WorldToScreen(rayHit.Position);

            bool isCenterRay = i == centerIndex;

            Color rayColor = isCenterRay ? Color.Yellow : new Color(0, 180, 80, 90);

            int thickness = isCenterRay ? 2 : 1;

            DrawLine(spriteBatch, start, end, rayColor, thickness);
        }
    }

    private void DrawPlayer(SpriteBatch spriteBatch, Player player)
    {
        int playerSize = 12;

        Vector2 screenPosition = WorldToScreen(player.Position);

        Rectangle playerRectangle = new Rectangle(
            (int)screenPosition.X - playerSize / 2,
            (int)screenPosition.Y - playerSize / 2,
            playerSize,
            playerSize
        );

        spriteBatch.Draw(_pixel, playerRectangle, Color.Red);
    }

    private void DrawPlayerDirection(SpriteBatch spriteBatch, Player player)
    {
        Vector2 start = WorldToScreen(player.Position);

        Vector2 direction = new Vector2(
            MathF.Cos(player.Angle),
            MathF.Sin(player.Angle)
        );

        Vector2 end = start + direction * 32f;

        DrawLine(spriteBatch, start, end, Color.Yellow, 3);
    }

    private Vector2 WorldToScreen(Vector2 worldPosition) => worldPosition * TILESIZE;

    private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        int thickness = 1;

        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, thickness), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Bottom - thickness, rectangle.Width, thickness), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Left, rectangle.Top, thickness, rectangle.Height), color);
        spriteBatch.Draw(_pixel, new Rectangle(rectangle.Right - thickness, rectangle.Top, thickness, rectangle.Height), color);
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
    {
        Vector2 edge = end - start;

        float angle = MathF.Atan2(edge.Y, edge.X);

        Rectangle lineRectangle = new Rectangle(
            (int)start.X,
            (int)start.Y,
            (int)edge.Length(),
            thickness
        );

        spriteBatch.Draw(_pixel, lineRectangle, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
    }
}
