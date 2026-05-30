/** Create three generated textures:
 * 1 = stone-like wall
 * 2 = brick-like wall
 * 3 = metal-like wall
 * Our current map already uses 1, 2, and 3, so this fits nicely
 * v0.12: Let's add procedural sprite textures:
 * SpriteId 1 = green enemy placeholder
 * SpriteId 2 = yellow pickup placeholder
 */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using WolfLike.src.Core;

namespace WolfLike.src.Graphics;

public class TextureManager
{
    private readonly Dictionary<int, Texture2D> _wallTextures = new();
    private readonly Dictionary<int, Texture2D> _spriteTextures = new();
    private readonly Dictionary<int, Texture2D> _weaponTextures = new();

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        int size = GameSettings.TEXTURESIZE;

        _wallTextures[1] = CreateStoneTexture(graphicsDevice, size, size);
        _wallTextures[2] = CreateBrickTexture(graphicsDevice, size, size);
        _wallTextures[3] = CreateMetalTexture(graphicsDevice, size, size);

        _spriteTextures[1] = CreateEnemyPlaceholderTexture(graphicsDevice, size, size);
        _spriteTextures[2] = CreatePickupPlaceholderTexture(graphicsDevice, size, size);
        _spriteTextures[3] = CreateAmmoPickupTexture(graphicsDevice, size, size);

        _weaponTextures[1] = CreateWeaponPlaceholderTexture(graphicsDevice, 160, 120);
        _weaponTextures[2] = CreateMuzzleFlashTexture(graphicsDevice, 96, 96);
    }

    public Texture2D GetWallTexture(int tileId)
    {
        if (_wallTextures.TryGetValue(tileId, out Texture2D texture))
            return texture;
        return _wallTextures[1];
    }

    public Texture2D GetSpriteTexture(int spriteId)
    {
        if (_spriteTextures.TryGetValue(spriteId, out Texture2D texture))
            return texture;
        return _spriteTextures[1];
    }

    public Texture2D GetWeaponTexture(int weaponTextureId)
    {
        if (_weaponTextures.TryGetValue(weaponTextureId, out Texture2D texture))
            return texture;
        return _weaponTextures[1];
    }

    private Texture2D CreateStoneTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0;  x < width; x++)
            {
                bool mortarLine = x % 16 == 0 || y % 16 == 0;

                Color color = mortarLine
                    ? new Color(80, 80, 90)
                    : new Color(150 + (x + y) % 25, 150 + (x * 2) % 20, 165 + (y * 3) % 20);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateBrickTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        int brickHeight = 16, brickWidth = 32;

        for (int y = 0; y < height; y++)
        {
            int row = y / brickHeight;
            int offset = row % 2 == 0 ? 0 : brickWidth / 2;

            for (int x = 0; x < width; x++)
            {
                int shiftedX = (x + offset) % width;

                bool mortarLine = y % brickHeight == 0 || shiftedX % brickWidth == 0;

                Color color = mortarLine
                    ? new Color(70, 45, 45)
                    : new Color(160 + (x % 20), 65 + (y % 20), 55 + ((x + y) % 15));

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateMetalTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool panelLine = x % 16 == 0 || y % 16 == 0;
                bool rivet = x % 16 == 8 || y % 16 == 8;

                Color color;

                if (rivet)
                    color = new Color(210, 220, 230);
                else if (panelLine)
                    color = new Color(45, 65, 85);
                else
                    color = new Color(75 + (x % 25), 115 + (y % 25), 150 + ((x + y) % 20));

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateEnemyPlaceholderTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        Vector2 center = new Vector2(width / 2f, height / 2f);
        float bodyRadius = width * 0.32f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0;  x < width; x++)
            {
                Vector2 position = new Vector2(x, y);
                float distance = Vector2.Distance(position, center);

                Color color = Color.Transparent;

                if (distance <= bodyRadius)
                {
                    float brightness = 1.0f - distance / bodyRadius * 0.45f;
                    color = new Color(
                        (byte)(60 * brightness),
                        (byte)(220 * brightness),
                        (byte)(90 * brightness),
                        (byte)255
                    );
                }

                bool eyeLeft = x >= 23 && x <= 28 && y >= 24 && y <= 30;
                bool eyeRight = x >= 36 && x <= 41 && y >= 24 && y <= 30;
                bool mouth = x >= 24 && x <= 40 && y >= 42 && y <= 45;

                if (eyeLeft || eyeRight || mouth)
                    color = new Color(10, 20, 10, 255);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreatePickupPlaceholderTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = Color.Transparent;

                bool vertical = x >= 28 && x <= 36 && y >= 16 && y <= 48;
                bool horizontal = y >= 28 && y <= 36 && x >= 16 && x <= 48;

                if (vertical || horizontal)
                    color = new Color(80, 230, 100, 255);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateWeaponPlaceholderTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = Color.Transparent;

                bool barrel = x >= 68 && x <= 92 && y >= 8 && y <= 72;

                bool barrelHighlight = x >= 74 && x <= 80 && y >= 12 && y <= 68;

                bool body = x >= 45 && x <= 115 && y >= 58 && y <= 105;

                bool grip = x >= 72 && x <= 102 && y >= 92 && y <= 119;

                bool handleCut = x >= 47 && x <= 68 && y >= 88 && y <= 112;

                if (barrel)
                    color = new Color(70, 70, 75, 255);
                if (barrelHighlight)
                    color = new Color(120, 120, 130, 255);
                if (body)
                    color = new Color(45, 45, 50, 255);
                if (grip)
                    color = new Color(35, 45, 40, 255);
                if (handleCut)
                    color = Color.Transparent;

                bool outline =
                    (x >= 43 && x <= 117 && y >= 56 && y <= 59) ||
                    (x >= 43 && x <= 117 && y >= 103 && y <= 106) ||
                    (x >= 43 && x <= 46 && y >= 56 && y <= 106) ||
                    (x >= 114 && x <= 117 && y >= 56 && y <= 106);

                if (outline)
                    color = new Color(10, 10, 12, 255);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateMuzzleFlashTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        Vector2 center = new Vector2(width / 2f, height / 2f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x, y);
                float distance = Vector2.Distance(position, center);

                Color color = Color.Transparent;

                bool verticalFlame = Math.Abs(x - center.X) < 10 && distance < 42;
                bool horizontalFlame = Math.Abs(y - center.Y) < 8 && distance < 34;
                bool diamond = Math.Abs(x - center.X) + Math.Abs(y - center.Y) < 38;

                if (verticalFlame || horizontalFlame || diamond)
                {
                    float brightness = 1.0f - distance / 48.0f;
                    brightness = MathHelper.Clamp(brightness, 0.0f, 1.0f);

                    color = new Color(
                        (byte)(255 * brightness),
                        (byte)(210 * brightness),
                        (byte)(60 * brightness),
                        (byte)220
                    );
                }

                if (distance < 12)
                    color = new Color(255, 245, 180, 255);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    private Texture2D CreateAmmoPickupTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color color = Color.Transparent;

                bool box =
                    x >= 14 && x <= 50 &&
                    y >= 22 && y <= 46;

                bool boxTop =
                    x >= 14 && x <= 50 &&
                    y >= 22 && y <= 27;

                bool stripe =
                    x >= 18 && x <= 46 &&
                    y >= 32 && y <= 36;

                bool outline =
                    (x >= 12 && x <= 52 && y >= 20 && y <= 22) ||
                    (x >= 12 && x <= 52 && y >= 46 && y <= 48) ||
                    (x >= 12 && x <= 14 && y >= 20 && y <= 48) ||
                    (x >= 50 && x <= 52 && y >= 20 && y <= 48);

                if (box) color = new Color(120, 95, 45, 255);
                if (boxTop) color = new Color(180, 145, 65, 255);
                if (stripe) color = new Color(230, 210, 90, 255);
                if (outline) color = new Color(35, 25, 10, 255);

                data[y * width + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }
}
