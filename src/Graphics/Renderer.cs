/**
 * In a Wolfenstein-style raycaster, objects/enemies/items are usually rendered as billboard sprites:
 * A 2D image placed at a 2D world position;
 * scaled by distance;
 * drawn on top of the 3D wall view;
 * and hidden behind walls when needed.
 * For learning purposes, let's add a simple procedural placeholder sprite and later, this can become an enemy or barrel or anything!
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using WolfLike.src.Entities;
using WolfLike.src.World;
using WolfLike.src.Core;
using System.Collections.Generic;
using System.Linq;

namespace WolfLike.src.Graphics;

public class Renderer
{
    private Texture2D _pixel;
    private TextureManager _textureManager;

    private const int TILESIZE = 48;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _textureManager = new();
        _textureManager.LoadContent(graphicsDevice);  // Now the renderer has access to wall textures
    }

    public void DrawRaycastView(SpriteBatch spriteBatch, WorldMap worldMap, Player player, RaycastHit[] rayHits, List<SpriteEntity> sprites)
    {
        //spriteBatch.Begin(blendState: BlendState.AlphaBlend);
        spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);  // PointClamp is important for retro pixel-style rendering
        // Without it, MonoGame may blur the texture columns when stretching them

        // The order matters:
        // ceiling / floor first
        // walls second
        // sprites third
        // Sprites are drawn after walls, but still a depth-check is needed so they do not appear through walls
        DrawCeilingAndFloor(spriteBatch);
        DrawWallSlices(spriteBatch, player, rayHits);
        DrawSprites(spriteBatch, player, rayHits, sprites);

        // Small debug minimap in the top-left corner
        //DrawMiniMap(spriteBatch, worldMap, player, rayHits);

        spriteBatch.End();
    }

    private void DrawCeilingAndFloor(SpriteBatch spriteBatch)
    {
        Rectangle ceilingRectangle = new Rectangle(0, 0, GameSettings.SCREENWIDTH, GameSettings.SCREENHEIGHT / 2);
        Rectangle floorRectangle = new Rectangle(0, GameSettings.SCREENHEIGHT / 2, GameSettings.SCREENWIDTH, GameSettings.SCREENHEIGHT / 2);

        spriteBatch.Draw(_pixel, ceilingRectangle, new Color(22, 24, 38));
        spriteBatch.Draw(_pixel, floorRectangle, new Color(38, 38, 42));
    }

    private void DrawWallSlices(SpriteBatch spriteBatch, Player player, RaycastHit[] rayHits)
    {
        if (rayHits == null || rayHits.Length == 0) return;

        int rayCount = rayHits.Length;
        float sliceWidth = (float)GameSettings.SCREENWIDTH / rayCount;  // Controls how wide each vertical slice is. If SCREENWIDTH=1280, rayCount=320 then sliceWidth = 4 pixels
        // So every ray draws a 4-pixel-wide vertical wall column

        for (int i = 0; i < rayCount; i++)
        {
            RaycastHit rayHit = rayHits[i];

            if (rayHit == null || !rayHit.HitWall) continue;

            // Without correction, the left and right sides of the screen look distorted because those rays travel diagonally
            // So let's correct the distance with fish-eye correction
            //float correctedDistance = CorrectFishEye(rayHit.Distance, rayHit.Angle, player.Angle);  // This avoids the classic fish-eye distortion
            // Without it, a flat wall in front of us may look curved. With it, walls look much more stable
            // But now it is not needed anymore, because we are using DDA, so rayHit.Distance is already corrected!
            float correctedDistance = rayHit.Distance;

            if (correctedDistance < 0.0001f)
                correctedDistance = 0.0001f;

            // The heart of the renderer
            float wallHeight = GameSettings.SCREENHEIGHT / correctedDistance;  // Objects closer to the camera appear larger. For example dist1 -> 720/1 = 720 pixels tall, dist2 -> 720/2 = 360 pixels tall
            // With this, we can create the 3D illusion

            int sliceX = (int)(i * sliceWidth);
            int sliceHeight = (int)wallHeight;
            int sliceY = GameSettings.SCREENHEIGHT / 2 - sliceHeight / 2;

            Rectangle destinationRectangle = new Rectangle(
                sliceX,
                sliceY,
                (int)MathF.Ceiling(sliceWidth) + 1,
                sliceHeight
            );

            DrawTexturedWallSlice(spriteBatch, rayHit, correctedDistance, destinationRectangle);
        }
    }

    private void DrawTexturedWallSlice(SpriteBatch spriteBatch, RaycastHit rayHit, float correctedDistance, Rectangle destinationRectangle)
    {
        Texture2D wallTexture = _textureManager.GetWallTexture(rayHit.TileId);

        int textureX = GetTextureXCoordinate(rayHit, wallTexture.Width);

        Rectangle sourceRectangle = new Rectangle(
            textureX,
            0,
            1,
            wallTexture.Height
        );

        Color shadeColor = GetTextureShadeColor(rayHit, correctedDistance);

        spriteBatch.Draw(wallTexture, destinationRectangle, sourceRectangle, shadeColor);
    }

    private void DrawSprites(SpriteBatch spriteBatch, Player player, RaycastHit[] rayHits, List<SpriteEntity> sprites)
    {
        if (sprites == null || sprites.Count == 0)
            return;

        // Why sort descending? Far sprites first, near sprites last
        List<SpriteEntity> sortedSprites = sprites
            .Where(sprite => sprite.IsVisible)
            .OrderByDescending(sprite => Vector2.DistanceSquared(player.Position, sprite.Position))
            .ToList();

        foreach (SpriteEntity sprite in sortedSprites)
            DrawSprite(spriteBatch, player, rayHits, sprite);
    }

    private void DrawSprite(SpriteBatch spriteBatch, Player player, RaycastHit[] rayHits, SpriteEntity sprite)
    {
        // This method projects the sprite from world space into screen space

        Vector2 toSprite = sprite.Position - player.Position;

        float distanceToSprite = toSprite.Length();  // Decides rendered size

        if (distanceToSprite < 0.0001f)
            return;

        float spriteAngle = MathF.Atan2(toSprite.Y, toSprite.X);
        float angleDifference = NormalizeAngle(spriteAngle - player.Angle);  // Decides left/right screen position

        float halfFov = GameSettings.FIELDOFVIEW / 2.0f;

        if (angleDifference < -halfFov || angleDifference > halfFov)
            return;

        float perpendicularDistance = distanceToSprite * MathF.Cos(angleDifference);

        if (perpendicularDistance <= 0.0001f)
            return;

        Texture2D spriteTexture = _textureManager.GetSpriteTexture(sprite.SpriteId);

        float screenXNormalized = 0.5f + angleDifference / GameSettings.FIELDOFVIEW;
        int spriteScreenCenterX = (int)(screenXNormalized * GameSettings.SCREENWIDTH);

        int projectedHeight = (int)(GameSettings.SCREENHEIGHT / perpendicularDistance * sprite.Scale);
        int projectedWidth = projectedHeight;

        int spriteTopY = GameSettings.SCREENHEIGHT / 2 - projectedHeight / 2;
        int spriteLeftX = spriteScreenCenterX - projectedWidth / 2;

        DrawSpriteWithDepthCheck(spriteBatch, spriteTexture, spriteLeftX, spriteTopY, projectedWidth, projectedHeight, perpendicularDistance, rayHits);
    }

    private void DrawSpriteWithDepthCheck(SpriteBatch spriteBatch, Texture2D spriteTexture, int spriteLeftX, int spriteTopY, int projectedWidth, int projectedHeight, float spriteDistance, RaycastHit[] rayHits)
    {
        // This methor draws the sprite one vertical column at a time
        // Why? Because each screen X position can have a different wall distance in front of it
        // This lets us hide parts of the sprite behind walls
        // This is the same basic concept old raycasters used with a depth buffer

        if (projectedWidth <= 0 || projectedHeight <= 0) 
            return;

        float rayToScreenScale = (float)rayHits.Length / GameSettings.SCREENWIDTH;

        for (int screenX = spriteLeftX; screenX < spriteLeftX + projectedWidth; screenX++)
        {
            if (screenX < 0 || screenX >= GameSettings.SCREENWIDTH)
                continue;

            int rayIndex = (int)(screenX * rayToScreenScale);
            rayIndex = Math.Clamp(rayIndex, 0, rayHits.Length - 1);

            RaycastHit wallHit = rayHits[rayIndex];

            if (wallHit != null && wallHit.HitWall && spriteDistance >= wallHit.Distance)
                continue;

            float textureRatio = (float)(screenX - spriteLeftX) / projectedWidth;
            int textureX = (int)(textureRatio * spriteTexture.Width);
            textureX = Math.Clamp(textureX, 0, spriteTexture.Width - 1);

            Rectangle sourceRectangle = new Rectangle(textureX, 0, 1, spriteTexture.Height);
            Rectangle destinationRectangle = new Rectangle(screenX, spriteTopY, 1, projectedHeight);

            Color shadeColor = GetSpriteShadeColor(spriteDistance);

            spriteBatch.Draw(spriteTexture, destinationRectangle, sourceRectangle, shadeColor);
        }
    }

    private Color GetSpriteShadeColor(float distance)
    {
        // This method makes far sprites darker and near sprites brighter

        float brightness = 1.0f / (1.0f + distance * 0.08f);

        brightness = MathHelper.Clamp(brightness, 0.25f, 1.0f);

        return new Color(brightness, brightness, brightness);
    }

    private float NormalizeAngle(float angle)
    {
        // This keeps angle differences in -PI to +PI range
        // Without this, looking across the 0 angle boundary can break sprite projection

        while (angle > MathF.PI)
            angle -= MathF.Tau;
        while (angle < -MathF.PI)
            angle += MathF.Tau;
        return angle;
    }

    private int GetTextureXCoordinate(RaycastHit rayHit, int textureWidth)
    {
        // This converts: WallX 0.0 to 1.0 into: texture column 0 to textureWidth -1
        // Example with 64-pixel-wide texture:
        // WallX = 0.00->textureX = 0
        // WallX = 0.50->textureX = 32
        // WallX = 0.99->textureX = 63

        float wallX = rayHit.WallX;

        int textureX = (int)(wallX * textureWidth);

        textureX = Math.Clamp(textureX, 0, textureWidth - 1);

        // When a ray hits the opposite side of a wall, the texture coordinate runs in the opposite direction
        // So we flip textureX for specific hit directions

        bool shouldFlipTexture =
            rayHit.HitSide == WallHitSide.Vertical && rayHit.RayDirection.X > 0 ||
            rayHit.HitSide == WallHitSide.Horizontal && rayHit.RayDirection.Y < 0;

        if (shouldFlipTexture)
            textureX = textureWidth - textureX - 1;

        return textureX;
    }

    private Color GetTextureShadeColor(RaycastHit rayHit, float correctedDistance)
    {
        float sideBrightness = rayHit.HitSide switch
        {
            WallHitSide.Vertical => 1.00f,
            WallHitSide.Horizontal => 0.68f,
            _ => 1.00f
        };

        float distanceBrightness = GetDistanceBrightness(correctedDistance);

        float finalBrightness = sideBrightness * distanceBrightness;

        // In Monogame, this acts as a tint multiplier
        // 1.0 = original texture color
        // 0.5 = half brightness
        // 0.2 = very dark
        // This lets the texture keep its color while becoming darker with distance
        return new Color(finalBrightness, finalBrightness, finalBrightness);
    }

    private float GetDistanceBrightness(float distance)
    {
        float brightness = 1.0f / (1.0f + distance * 0.12f);

        return MathHelper.Clamp(brightness, 0.18f, 1.0f);
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
