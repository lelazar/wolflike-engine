/** Create three generated textures:
 * 1 = stone-like wall
 * 2 = brick-like wall
 * 3 = metal-like wall
 * Our current map already uses 1, 2, and 3, so this fits nicely
 */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WolfLike.src.Graphics;

public class TextureManager
{
    private readonly Dictionary<int, Texture2D> _wallTextures = new();

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _wallTextures[1] = CreateStoneTexture(graphicsDevice, 64, 64);
        _wallTextures[2] = CreateBrickTexture(graphicsDevice, 64, 64);
        _wallTextures[3] = CreateMetalTexture(graphicsDevice, 64, 64);
    }

    public Texture2D GetWallTexture(int tileId)
    {
        if (_wallTextures.TryGetValue(tileId, out Texture2D texture))
            return texture;
        return _wallTextures[1];
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
}
